using UnityEngine;

public class PlayerControllerOld : MonoBehaviour
{
    [Header("Player Properties")]
    [Tooltip("Скорость движения игрока")] [SerializeField] private float _moveSpeed; // Скорость персонажа
    [Tooltip("Сила прыжка")] [SerializeField] private float _jumpForce; // Сила прыжка
    [Tooltip("Чувствительность мыши")] [SerializeField] private float _turnSpeed; // Сенса
    [Tooltip("Максимальный угол поворота камеры")] [SerializeField] private float _maxAngle; // Максимальный угол поворота для камеры
    [Tooltip("Минимальный угол поворота камеры")] [SerializeField] private float _minAngle; // Минимальный угол поворота для камеры
    private Rigidbody _rigidbody;
    private Camera _camera;
    private Vector3 _targetDir;
    private float _camRotation;
    private float _angle;
    private float _vertivalInput;
    private float _horizontalInput;
    private float _mouseHorizontalInput;
    private float _mouseVerticalInput;
    private bool _jumpInput;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        GetInput();
        PlayerMove();
        PlayerRotate();
        CamRotate();
    }

    private void GetInput()
    {
        _vertivalInput = Input.GetAxis("Vertical") * _moveSpeed * Time.deltaTime;
        _horizontalInput = Input.GetAxis("Horizontal") * _moveSpeed * Time.deltaTime;
        _mouseHorizontalInput = Input.GetAxis("Mouse X") * _turnSpeed * Time.deltaTime;
        _mouseVerticalInput = Input.GetAxis("Mouse Y") * _turnSpeed * Time.deltaTime;
    }

    private void PlayerMove()
    {
        transform.Translate(Vector3.forward * _vertivalInput);
        transform.Translate(Vector3.right * _horizontalInput);
    }

    private void PlayerRotate() => transform.Rotate(Vector3.up * _mouseHorizontalInput);
    private void CamRotate() 
    {
        CamCropAngle();
        _camera.transform.localRotation = Quaternion.Euler(_camRotation, 0f, 0f);
    }
    
    private void CamCropAngle()
    {
        _camRotation -= _mouseVerticalInput;
        _camRotation = Mathf.Clamp(_camRotation, -_maxAngle, _minAngle);
    }
}
