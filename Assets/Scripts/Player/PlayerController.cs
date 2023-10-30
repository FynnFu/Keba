using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Обрезка угла наклона камеры")]
    [SerializeField, Tooltip("Максимальный угол поворота камеры"), Range(70, 90)] private int _maxAngle; 
    [SerializeField, Tooltip("Минимальный угол поворота камеры"), Range(70, 90)] private int _minAngle;

    [Header("Чувствительность")]
    [SerializeField, Tooltip("Чувствительность мыши"), Range(1, 900)] private int _turnSpeed;

    [Header("Скорость")]
    [SerializeField, Tooltip("Скорость персонажа по умолчанию"), Range(1f, 10f)] private float _defaultSpeed; 
    [SerializeField, Tooltip("Скорость спринта персонажа"), Range(1f, 20f)] private float _sprintSpeed; 
    [SerializeField, Tooltip("Ускорение персонажа"), Range(1f, 40f)] private float _accelerationSpeed;

    [Header("Коллайдер")]
    [SerializeField, Tooltip("Стандартная высота колайдера"), Range(0.1f, 3f)] private float _defaultColliderHeight;

    [Header("Приседание")]
    [SerializeField, Tooltip("Замедление в приседе ( Делитель )"), Range(0.1f, 3f)] float _squatSlowdown;
    [SerializeField, Tooltip("Высота коллайдера в приседе"), Range(0.1f, 3f)] private float _squatColliderHeight;
    [SerializeField, Tooltip("Длина луча для приседания"), Range(-1f, 2f)] private float _rayMaxLength;
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
    private float _time;
    private bool _isSquat;
    private bool _isSprint;
    private bool _isRayHit;
    private bool _isInteract;

    private const float _GravityStrenght = 9.87f;
    private const int _CollPosDivider = 2;

    public float VerticalInput { get => _moveInput.y; }
    public float HorizontalInput { get => _moveInput.x; }
    public float CharacterSpeed { get => _characterSpeed; }
    public bool IsInteract { get => _isInteract; }
    public bool IsSquat { get => _isSquat; }
    public bool IsRayHit { get => _isRayHit; }

    private void Awake() => _playerControls = new PlayerControls();
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
        SpeedRegulate();
        PlayerMove(_moveInput);
        PlayerRotate(_rotateInput.x);
        CamRotate(_rotateInput.y);
        SquatCollider(_isSquat);
        IsGrounded();
    }

    private void GetInput()
    {
        _moveInput = _playerControls.Player.Move.ReadValue<Vector2>() * _characterSpeed * Time.deltaTime; 
        _rotateInput = _playerControls.Player.Rotate.ReadValue<Vector2>() * _turnSpeed * Time.deltaTime;
        _isInteract = _playerControls.Player.ObjInteraction.WasPressedThisFrame();
        _isSquat = _playerControls.Player.Squat.IsPressed() || _isRayHit;
        _isSprint = _playerControls.Player.Sprint.IsPressed() && !_isSquat;
        /*_isSquat = _playerControls.Player.Squat.IsPressed() || _isRayHit;*/
    }

    private void PlayerMove(Vector2 moveInput)
    {
        _moveY = Mathf.Lerp(_moveY, moveInput.y, Time.deltaTime * _accelerationSpeed);
        _moveX = Mathf.Lerp(_moveX, moveInput.x, Time.deltaTime * _accelerationSpeed);
        _move = ((_moveY * transform.forward) + (_moveX * transform.right)); // Вектор движения
        _characterController.Move(_move); // Движение в пространствве
        _characterController.SimpleMove(Vector3.down * _GravityStrenght); // Гравитация
    }

    private void PlayerRotate(float inputValue) => transform.Rotate(Vector3.up * inputValue); // Ротация игрока по горизонтали

    private void CamRotate(float inputValue) => _camera.transform.localRotation = Quaternion.Euler(CamCropAngle(inputValue), 0f, 0f);

    private float CamCropAngle(float valueToCrop) // Обрезка наклона камеры до максимального и минимального 
    {
        _camRotation -= valueToCrop;
        _camRotation = Mathf.Clamp(_camRotation, -_maxAngle, _minAngle);
        return _camRotation;
    }

    private void SpeedRegulate()
    {
        if (_isSquat) _characterSpeed = _defaultSpeed / _squatSlowdown; // Замедление при приседании
        else _characterSpeed = SprintActivationDelay(_isSprint, 1f, ref _time) ? _sprintSpeed : _defaultSpeed;


        bool SprintActivationDelay(bool inputVal, float delay, ref float timer)
        {
            if (Time.time > timer)
            {
                if (inputVal)
                {
                    return true;
                }
                else timer = Time.time + delay;
                return false;
            }
            return false;
        }
        /*if (_isSquat) _characterSpeed = _defaultSpeed / _squatSlowdown; // Замедление при приседании
        else _characterSpeed = _isSprint ? _sprintSpeed : _defaultSpeed;*/
    }
    private void SquatCollider(bool toggleVal) // Поведение коллайдера в приседе
    {
        _characterController.height = toggleVal ? _squatColliderHeight : _defaultColliderHeight;
        _characterController.center = toggleVal ? _squatColliderPos : _defaultColliderPos;
        _rayOrigin = transform.position;
        _isRayHit = Physics.Raycast(_rayOrigin, Vector3.up, _rayMaxLength + _characterController.height / _CollPosDivider);
        Debug.DrawRay(_rayOrigin, Vector3.up, Color.red, _rayMaxLength + _characterController.height / _CollPosDivider);
    }

    private void IsGrounded() => _audioManager.enabled = _characterController.isGrounded;
}