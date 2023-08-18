using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Максимальный угол поворота камеры")][SerializeField] private float _maxAngle; 
    [Tooltip("Минимальный угол поворота камеры")][SerializeField] private float _minAngle; 
    [Tooltip("Чувствительность мыши")][SerializeField] private float _turnSpeed;
    [Tooltip("Скорость персонажа по умолчанию")][SerializeField] private float _defaultSpeed; 
    [Tooltip("Скорость спринта персонажа")][SerializeField] private float _sprintSpeed; 
    [Tooltip("Ускорение персонажа")][SerializeField] private float _accelerationSpeed; 
    [Tooltip("Стандартная высота колайдера")][SerializeField] private float _defaultColliderHeight; 
    [Tooltip("Высота коллайдера при приседании")][SerializeField] private float _squatColliderHeight; 
    [Tooltip("Длина луча для приседания")][SerializeField] private float _rayMaxLength; 
    //  Позволяет понять отключать ли приседание, если персонах под объектом

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

    private float _characterSpeed;
    private float _camRotation;
    private float _moveY;
    private float _moveX;
    private bool _isSquat;
    private bool _isRayHit;
    private bool _isInteract;

    private const float _gravityStrenght = 9.87f;
    private const float _CollPosDivider = 2f;

    public float VerticalInput { get => _moveInput.y; }
    public float HorizontalInput { get => _moveInput.x; }
    public float CharacterSpeed { get => _characterSpeed; }
    public bool IsInteract { get => _isInteract; }
    public bool IsSquat { get => _isSquat; }
    public bool IsRayHit { get => _isRayHit; }

    private void Awake() =>_playerControls = new PlayerControls();
    private void OnEnable() => _playerControls.Player.Enable();
    private void OnDisable() => _playerControls.Player.Disable();

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _audioManager = FindObjectOfType<AudioManager>();
        _defaultColliderPos = new Vector3(0, 0, 0);
        _squatColliderPos = new Vector3(0, -(_defaultColliderHeight - _squatColliderHeight) / _CollPosDivider, 0);
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
        _isSquat = _playerControls.Player.Squat.IsPressed() || _isRayHit;
        _characterSpeed = (_playerControls.Player.Sprint.IsPressed() && !_isSquat) ? _sprintSpeed : _defaultSpeed;
    }

    private void PlayerRotate() => transform.Rotate(Vector3.up * _rotateInput.x); // Ротация игрока по горизонтали

    private void PlayerMove()
    {
        SquatCollider();
        _moveY = Mathf.Lerp(_moveY, _moveInput.y, Time.deltaTime * _accelerationSpeed);
        _moveX = Mathf.Lerp(_moveX, _moveInput.x, Time.deltaTime * _accelerationSpeed);
        _move = ((_moveY * transform.forward) + (_moveX * transform.right)); // Вектор движения
        _characterController.Move(_move); // Движение в пространствве
        _characterController.SimpleMove(Vector3.down * _gravityStrenght); // Гравитация
    }

    private void CamRotate() => _camera.transform.localRotation = Quaternion.Euler(CamCropAngle(), 0f, 0f);

    private float CamCropAngle() // Обрезка наклона камеры до максимального и минимального 
    {
        _camRotation -= _rotateInput.y;
        _camRotation = Mathf.Clamp(_camRotation, -_maxAngle, _minAngle);
        return _camRotation;
    }

    private void SquatCollider() // Поведение коллайдера в приседе
    {
        _rayOrigin = transform.position;
        _isRayHit = Physics.Raycast(_rayOrigin, Vector3.up, _rayMaxLength + _rayMaxLength + _characterController.height / 2);
        _characterController.height = _isSquat ? _squatColliderHeight : _defaultColliderHeight;
        _characterController.center = _isSquat ? _squatColliderPos : _defaultColliderPos;
    }

    private void IsGrounded() => _audioManager.enabled = _characterController.isGrounded;
}