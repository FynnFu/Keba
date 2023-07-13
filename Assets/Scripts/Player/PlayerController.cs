using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Максимальный угол поворота камеры")][SerializeField] private float _maxAngle; // Максимальный угол поворота для камеры
    [Tooltip("Минимальный угол поворота камеры")][SerializeField] private float _minAngle; // Минимальный угол поворота для камеры
    [Tooltip("Чувствительность мыши")][SerializeField] private float _turnSpeed; // Сенса
    [Tooltip("Скорость персонажа")][SerializeField] private float _defaultSpeed;
    [Tooltip("Скорость спринтаа персонажа")][SerializeField] private float _sprintSpeed;
    [Tooltip("Стандартная высота колайдера")][SerializeField] private float _defaultColliderHeight;
    [Tooltip("Высота коллайдера при приседании")][SerializeField] private float _squatColliderHeight;
    [Tooltip("Позиция коллайдера при приседании")][SerializeField] private float _squatColliderPos;
    private CharacterController _characterController;
    private Camera _camera;
    private AudioManager _audioManager;
    /*private CapsuleCollider _capsuleCollider;*/
    private Vector3 _move;
    private float _characterSpeed;
    private float _camRotation;
    private float _verticalInput;
    private float _horizontalInput;
    private float _mouseHorizontalInput;
    private float _mouseVerticalInput;
    private float _gravityStrenght = 9.87f;
    private bool _isSquat;
    public bool IsSquat { get => _isSquat; }
    public float VerticalInput { get => _verticalInput; }
    public float HorizontalInput { get => _horizontalInput; }
    public float CharacterSpeed { get => _characterSpeed; }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _audioManager = FindObjectOfType<AudioManager>();
        /*_capsuleCollider = GetComponent<CapsuleCollider>();*/
        Cursor.visible = false;
    }

    private void Update()
    {
        GetInput();
        PlayerMove();
        PlayerRotate();
        CamRotate();
        IsGrounded();
    }

    private void GetInput()
    {
        _isSquat = Input.GetKey(KeyCode.LeftControl);
        _characterSpeed = Input.GetKey(KeyCode.LeftShift) ? _sprintSpeed : _defaultSpeed;
        _verticalInput = Input.GetAxis("Vertical") * _characterSpeed * Time.deltaTime;
        _horizontalInput = Input.GetAxis("Horizontal") * _characterSpeed * Time.deltaTime;
        _mouseHorizontalInput = Input.GetAxis("Mouse X") * _turnSpeed * Time.deltaTime;
        _mouseVerticalInput = Input.GetAxis("Mouse Y") * _turnSpeed * Time.deltaTime;
    }


    private void PlayerRotate() => transform.Rotate(Vector3.up * _mouseHorizontalInput);

    private void PlayerMove()
    {
        SquatCollider();
        _move = (_verticalInput * transform.forward) + (_horizontalInput * transform.right); // Вектор движения
        _characterController.Move(_move); // Движение в пространствве
        _characterController.SimpleMove(Vector3.down * _gravityStrenght); // Гравитация
    }

    private void CamRotate() => _camera.transform.localRotation = Quaternion.Euler(CamCropAngle(), 0f, 0f);


    private float CamCropAngle()
    {
        _camRotation -= _mouseVerticalInput;
        _camRotation = Mathf.Clamp(_camRotation, -_maxAngle, _minAngle);
        return _camRotation;
    }

    private void SquatCollider()
    {
        if (_isSquat)
        {
            _characterController.height = _squatColliderHeight;
            _characterController.center = new Vector3(0, _squatColliderPos, 0);
        } 
        else
        {
            _characterController.height = _defaultColliderHeight;
            _characterController.center = new Vector3(0,0,0);
        }
    }

    private void IsGrounded() => _audioManager.enabled = _characterController.isGrounded;
}