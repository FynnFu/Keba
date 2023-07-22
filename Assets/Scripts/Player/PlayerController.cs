using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Максимальный угол поворота камеры")][SerializeField] private float _maxAngle; // Максимальный угол поворота для камеры
    [Tooltip("Минимальный угол поворота камеры")][SerializeField] private float _minAngle; // Минимальный угол поворота для камеры
    [Tooltip("Чувствительность мыши")][SerializeField] private float _turnSpeed; // Сенса
    [Tooltip("Скорость персонажа")][SerializeField] private float _defaultSpeed;
    [Tooltip("Скорость спринтаа персонажа")][SerializeField] private float _sprintSpeed;
    [Tooltip("Стандартная высота колайдера")][SerializeField] private float _defaultColliderHeight;
    [Tooltip("Высота коллайдера при приседании")][SerializeField] private float _squatColliderHeight;
    [SerializeField] private float _rayMaxDist;

    private CharacterController _characterController;
    private PlayerInput _input;
    private Camera _camera;
    private AudioManager _audioManager;
    private Vector3 _inputAxis;
    private Vector2 _inputMouseAxis;
    private Vector3 _move;
    private Vector3 _defaultColliderPos;
    private Vector3 _squatColliderPos;
    private Vector3 _rayOrigin;
    /*private Vector3 _rayDirection;*/

    private float _characterSpeed;
    private float _camRotation;
    private float _verticalInput;
    private float _horizontalInput;
    private float _mouseHorizontalInput;
    private float _mouseVerticalInput;
    private bool _isSquat;

    private const float _gravityStrenght = 9.87f;
    private const float _CollPosDivider = 2f;

    public bool IsSquat { get => _isSquat; }
    public float VerticalInput { get => _inputAxis.z; }
    public float HorizontalInput { get => _inputAxis.x; }
    public float CharacterSpeed { get => _characterSpeed; }

    private void Awake() => GetComponent<PlayerInput>();
    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _audioManager = FindObjectOfType<AudioManager>();

        _defaultColliderPos = new Vector3(0, 0, 0);
        _squatColliderPos = new Vector3(0, -(_defaultColliderHeight - _squatColliderHeight) / _CollPosDivider, 0);

        _rayOrigin = new Vector3(0, 0, 0);

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
        /*_isSquat = Input.GetKey(KeyCode.LeftControl);
        _characterSpeed = Input.GetKey(KeyCode.LeftShift) ? _sprintSpeed : _defaultSpeed;
        _verticalInput = Input.GetAxis("Vertical") * _characterSpeed * Time.deltaTime;
        _horizontalInput = Input.GetAxis("Horizontal") * _characterSpeed * Time.deltaTime;
        _mouseHorizontalInput = Input.GetAxis("Mouse X") * _turnSpeed * Time.deltaTime;
        _mouseVerticalInput = Input.GetAxis("Mouse Y") * _turnSpeed * Time.deltaTime;*/
        _inputAxis = _input.Player.Move.ReadValue<Vector3>() * _defaultSpeed * Time.deltaTime;
        _inputMouseAxis = _input.Player.Rotate.ReadValue<Vector2>() * _turnSpeed * Time.deltaTime;
    }


    private void PlayerRotate() => transform.Rotate(Vector3.up * _inputMouseAxis.x);

    private void PlayerMove()
    {
        SquatCollider();
        _move = _inputAxis; // Вектор движения
        _characterController.Move(_move); // Движение в пространствве
        _characterController.SimpleMove(Vector3.down * _gravityStrenght); // Гравитация
    }

    private void CamRotate() => _camera.transform.localRotation = Quaternion.Euler(CamCropAngle(), 0f, 0f);


    private float CamCropAngle()
    {
        _camRotation -= _inputMouseAxis.y;
        _camRotation = Mathf.Clamp(_camRotation, -_maxAngle, _minAngle);
        return _camRotation;
    }

    private void SquatCollider()
    {
        Debug.Log(Physics.Raycast(_rayOrigin, Vector3.forward, _rayMaxDist));
        Debug.DrawRay(_rayOrigin, Vector3.forward * _rayMaxDist, Color.green);
        if (_isSquat)
        {
            _characterController.height = _squatColliderHeight;
            _characterController.center = _squatColliderPos;
        }
        else
        {
            _characterController.height = _defaultColliderHeight;
            _characterController.center = _defaultColliderPos;
        }
    }

    private void IsGrounded() => _audioManager.enabled = _characterController.isGrounded;
}



/*using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Максимальный угол поворота камеры")][SerializeField] private float _maxAngle; // Максимальный угол поворота для камеры
    [Tooltip("Минимальный угол поворота камеры")][SerializeField] private float _minAngle; // Минимальный угол поворота для камеры
    [Tooltip("Чувствительность мыши")][SerializeField] private float _turnSpeed; // Сенса
    [Tooltip("Скорость персонажа")][SerializeField] private float _defaultSpeed;
    [Tooltip("Скорость спринтаа персонажа")][SerializeField] private float _sprintSpeed;
    [Tooltip("Стандартная высота колайдера")][SerializeField] private float _defaultColliderHeight;
    [Tooltip("Высота коллайдера при приседании")][SerializeField] private float _squatColliderHeight;
    [SerializeField] private float _rayMaxDist;

    private CharacterController _characterController;
    private PlayerInput _input;
    private Camera _camera;
    private AudioManager _audioManager;
    private Vector3 _inputAxis;
    private Vector2 _inputMouseAxis;
    private Vector3 _move;
    private Vector3 _defaultColliderPos;
    private Vector3 _squatColliderPos;
    private Vector3 _rayOrigin;
    *//*private Vector3 _rayDirection;*//*

    private float _characterSpeed;
    private float _camRotation;
    private float _verticalInput;
    private float _horizontalInput;
    private float _mouseHorizontalInput;
    private float _mouseVerticalInput;
    private bool _isSquat;

    private const float _gravityStrenght = 9.87f;
    private const float _CollPosDivider = 2f;

    public bool IsSquat { get => _isSquat; }
    public float VerticalInput { get => _inputAxis.z; }
    public float HorizontalInput { get => _inputAxis.x; }
    public float CharacterSpeed { get => _characterSpeed; }

    private void Awake() => _input = new PlayerInput();
    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _audioManager = FindObjectOfType<AudioManager>();

        _defaultColliderPos = new Vector3(0, 0, 0);
        _squatColliderPos = new Vector3(0, -(_defaultColliderHeight - _squatColliderHeight) / _CollPosDivider, 0);

        _rayOrigin = new Vector3(0, 0, 0);

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
        *//*_isSquat = Input.GetKey(KeyCode.LeftControl);
        _characterSpeed = Input.GetKey(KeyCode.LeftShift) ? _sprintSpeed : _defaultSpeed;
        _verticalInput = Input.GetAxis("Vertical") * _characterSpeed * Time.deltaTime;
        _horizontalInput = Input.GetAxis("Horizontal") * _characterSpeed * Time.deltaTime;
        _mouseHorizontalInput = Input.GetAxis("Mouse X") * _turnSpeed * Time.deltaTime;
        _mouseVerticalInput = Input.GetAxis("Mouse Y") * _turnSpeed * Time.deltaTime;*//*
        _characterSpeed = Input.GetKey(KeyCode.LeftShift) ? _sprintSpeed : _defaultSpeed;
        _inputAxis = _input.Player.Move.ReadValue<Vector3>() * _characterSpeed * Time.deltaTime;
        _inputMouseAxis = _input.Player.Rotate.ReadValue<Vector2>() * _turnSpeed * Time.deltaTime;
    }


    private void PlayerRotate() => transform.Rotate(Vector3.up * _inputMouseAxis.x);

    private void PlayerMove()
    {
        SquatCollider();
        _move = _inputAxis; // Вектор движения
        _characterController.Move(_move); // Движение в пространствве
        _characterController.SimpleMove(Vector3.down * _gravityStrenght); // Гравитация
    }

    private void CamRotate() => _camera.transform.localRotation = Quaternion.Euler(CamCropAngle(), 0f, 0f);


    private float CamCropAngle()
    {
        _camRotation -= _inputMouseAxis.y;
        _camRotation = Mathf.Clamp(_camRotation, -_maxAngle, _minAngle);
        return _camRotation;
    }

    private void SquatCollider()
    {
        Debug.Log(Physics.Raycast(_rayOrigin, Vector3.forward, _rayMaxDist));
        Debug.DrawRay(_rayOrigin, Vector3.forward * _rayMaxDist, Color.green);
        if (_isSquat)
        {
            _characterController.height = _squatColliderHeight;
            _characterController.center = _squatColliderPos;
        }
        else
        {
            _characterController.height = _defaultColliderHeight;
            _characterController.center = _defaultColliderPos;
        }
    }

    private void IsGrounded() => _audioManager.enabled = _characterController.isGrounded;
}*/