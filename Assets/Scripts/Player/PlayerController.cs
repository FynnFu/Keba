using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Максимальный угол поворота камеры")][SerializeField] private float _maxAngle; // Максимальный угол поворота для камеры
    [Tooltip("Минимальный угол поворота камеры")][SerializeField] private float _minAngle; // Минимальный угол поворота для камеры
    [Tooltip("Чувствительность мыши")][SerializeField] private float _turnSpeed; // Сенса
    [Tooltip("Скорость персонажа по умолчанию")][SerializeField] private float _defaultSpeed; // Скорость персонажа по умолчанию
    [Tooltip("Скорость спринта персонажа")][SerializeField] private float _sprintSpeed; // Скорость спринта персонажа
    [Tooltip("Стандартная высота колайдера")][SerializeField] private float _defaultColliderHeight; // Стандартная высота колайдера
    [Tooltip("Высота коллайдера при приседании")][SerializeField] private float _squatColliderHeight; // Высота коллайдера при приседании
    [Tooltip("Длина луча для приседания")][SerializeField] private float _rayMaxLength; // Позволяет понять отключать ли приседание, если персонах под объектом

    private PlayerControls _playerControls;
    private CharacterController _characterController;
    private Camera _camera;
    private AudioManager _audioManager;
    private Vector2 _moveInput;
    private Vector2 _rotateInput;
    private Vector3 _move;
    private Vector3 _defaultColliderPos;
    private Vector3 _squatColliderPos;
    private Vector3 _rayOrigin; 
    /*private Vector3 _rayDirection;*/

    private float _characterSpeed;
    private float _camRotation;
    private bool _isSquat;
    private bool _isInteract;
    private bool _isRayHit;

    private const float _gravityStrenght = 9.87f;
    private const float _CollPosDivider = 2f;

    public float VerticalInput { get => _moveInput.y; }
    public float HorizontalInput { get => _moveInput.x; }
    public bool IsInteract { get => _isInteract; }
    public bool IsSquat { get => _isSquat; }
    public bool IsRayHit { get => _isRayHit; }
    public float CharacterSpeed { get => _characterSpeed; }

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    private void OnEnable() => _playerControls.Player.Enable();
    private void OnDisable() => _playerControls.Player.Disable();

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _audioManager = FindObjectOfType<AudioManager>();

        _defaultColliderPos = new Vector3(0, 0, 0);
        _squatColliderPos = new Vector3(0, -(_defaultColliderHeight - _squatColliderHeight) / _CollPosDivider, 0);

        /*_rayOrigin = new Vector3(transform.position.x, transform.position.y, transform.position.z);*/
    }

    private void Update()
    {
        Cursor.visible = false;
        GetInput();
        PlayerMove();
        PlayerRotate();
        CamRotate();
        IsGrounded();
    }

    private void GetInput()
    {
        _moveInput = _playerControls.Player.Move.ReadValue<Vector2>() * _characterSpeed * Time.deltaTime;
        _rotateInput = _playerControls.Player.Rotate.ReadValue<Vector2>() * _turnSpeed * Time.deltaTime;
        _isInteract = _playerControls.Player.ObjInteraction.WasPressedThisFrame();
        _isSquat = _playerControls.Player.Squat.IsPressed();
        _characterSpeed = (_playerControls.Player.Sprint.IsPressed() && !(_isSquat || _isRayHit)) ? _sprintSpeed : _defaultSpeed;
    }


    private void PlayerRotate() => transform.Rotate(Vector3.up * _rotateInput.x);

    private void PlayerMove()
    {
        SquatCollider();
        _move = ((_moveInput.y * transform.forward) + (_moveInput.x * transform.right)); // Вектор движения
        _characterController.Move(_move); // Движение в пространствве
        _characterController.SimpleMove(Vector3.down * _gravityStrenght); // Гравитация
    }

    private void CamRotate() => _camera.transform.localRotation = Quaternion.Euler(CamCropAngle(), 0f, 0f);


    private float CamCropAngle()
    {
        _camRotation -= _rotateInput.y;
        _camRotation = Mathf.Clamp(_camRotation, -_maxAngle, _minAngle);
        return _camRotation;
    }

    private void SquatCollider()
    {
        _rayOrigin = transform.position;
        _isRayHit = Physics.Raycast(_rayOrigin, Vector3.up, _rayMaxLength + _rayMaxLength + _characterController.height / 2);
        _characterController.height = (_isSquat || _isRayHit) ? _squatColliderHeight : _defaultColliderHeight;
        _characterController.center = (_isSquat || _isRayHit) ? _squatColliderPos : _defaultColliderPos;
        Debug.DrawRay(_rayOrigin, Vector3.up * (_rayMaxLength + _characterController.height / 2), Color.red);
        Debug.Log(_isRayHit);
    }

    private void IsGrounded() => _audioManager.enabled = _characterController.isGrounded;
}