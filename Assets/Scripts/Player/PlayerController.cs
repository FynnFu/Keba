using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Максимальный угол поворота камеры")][SerializeField] private float _maxAngle; // Максимальный угол поворота для камеры
    [Tooltip("Минимальный угол поворота камеры")][SerializeField] private float _minAngle; // Минимальный угол поворота для камеры
    [Tooltip("Чувствительность мыши")][SerializeField] private float _turnSpeed; // Сенса
    [SerializeField] private float _characterSpeed;
    private CharacterController _characterController;
    private Camera _camera;
    private Vector3 _move;
    private float _camRotation;
    private float _vertivalInput;
    private float _horizontalInput;
    private float _mouseHorizontalInput;
    private float _mouseVerticalInput;
    private float _gravityStrenght = 9.87f;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        Cursor.visible = false;
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
        _vertivalInput = Input.GetAxis("Vertical") * _characterSpeed * Time.deltaTime;
        _horizontalInput = Input.GetAxis("Horizontal") * _characterSpeed * Time.deltaTime;
        _mouseHorizontalInput = Input.GetAxis("Mouse X") * _turnSpeed * Time.deltaTime;
        _mouseVerticalInput = Input.GetAxis("Mouse Y") * _turnSpeed * Time.deltaTime;
    }


    private void PlayerRotate() => transform.Rotate(Vector3.up * _mouseHorizontalInput);

    private void PlayerMove()
    {
        _move = (_vertivalInput * transform.forward) + (_horizontalInput * transform.right); // Вектор движения
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
}