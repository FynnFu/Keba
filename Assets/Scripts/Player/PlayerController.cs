using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Максимальный угол поворота камеры")]
    [SerializeField] private float _maxAngle; // Максимальный угол поворота для камеры
    [Tooltip("Минимальный угол поворота камеры")]
    [SerializeField] private float _minAngle; // Минимальный угол поворота для камеры
    [Tooltip("Чувствительность мыши")]
    [SerializeField] private float _turnSpeed; // Сенса
    [SerializeField] private float _characterSpeed;
    [SerializeField] private float _cameraSwingSpeed; // Скорость покачивания камеры
    [SerializeField] private float _cameraSwingAmount; // Амплитуда покачивания камеры
    private CharacterController _characterController;
    private Camera _camera;
    private Vector3 _camDeffaultPos;
    private Vector3 _move;
    private float _camRotation;
    private float _verticalInput;
    private float _horizontalInput;
    private float _mouseHorizontalInput;
    private float _mouseVerticalInput;
    private float _gravityStrength = 9.87f;
    private float _cameraSwingOffset;
    private bool _isMoving;
    private bool _isColliding;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        Cursor.visible = false;
        _cameraSwingOffset = 0f; // Инициализация смещения покачивания камеры
        _camDeffaultPos = new Vector3(_camera.transform.localPosition.y, _camera.transform.localPosition.y, _camera.transform.localPosition.y);
    }

    private void Update()
    {
        GetInput();
        PlayerMove();
        PlayerRotate();
        CamRotate();
    }

    private void GetInput()
    {
        _verticalInput = Input.GetAxis("Vertical");
        _horizontalInput = Input.GetAxis("Horizontal");
        _mouseHorizontalInput = Input.GetAxis("Mouse X") * _turnSpeed * Time.deltaTime;
        _mouseVerticalInput = Input.GetAxis("Mouse Y") * _turnSpeed * Time.deltaTime;

        // Проверяем, если персонаж движется, чтобы активировать покачивание камеры
        _isMoving = Mathf.Abs(_verticalInput) > 0f || Mathf.Abs(_horizontalInput) > 0f;
    }

    private void PlayerRotate()
    {
        Vector3 rotation = new Vector3(0f, _mouseHorizontalInput, 0f);
        transform.Rotate(rotation);
    }

    private void PlayerMove()
    {
        _move = (_verticalInput * transform.forward) + (_horizontalInput * transform.right);
        _move.Normalize();
        _characterController.Move(_move * _characterSpeed * Time.deltaTime);
        _characterController.SimpleMove(Vector3.down * _gravityStrength);
    }

    private void CamRotate()
    {
        _camRotation -= _mouseVerticalInput;
        _camRotation = Mathf.Clamp(_camRotation, -_maxAngle, _minAngle);

        // Рассчитываем смещение покачивания камеры при передвижении
        if (_isMoving)
        {
            _cameraSwingOffset = Mathf.Sin(Time.time * _cameraSwingSpeed) * _cameraSwingAmount;
        }
        else
        {
            // Плавное затухание покачивания камеры при остановке персонажа
            _cameraSwingOffset = Mathf.Lerp(_cameraSwingOffset, 0f, Time.deltaTime * _cameraSwingSpeed);
        }

        Debug.Log(_cameraSwingOffset);

        _camera.transform.localRotation = UnityEngine.Quaternion.Euler(_camRotation, 0f, 0f);
        _camera.transform.localPosition = new Vector3(0, (_camDeffaultPos.y + _cameraSwingOffset / 10), 0);
    }
}
