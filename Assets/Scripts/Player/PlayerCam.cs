using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Tooltip("Скорость покачивания камеры")][SerializeField] private float _cameraSwingMultiplier;
    [Tooltip("Дистанция покачивания")][SerializeField] private float _cameraSwingAmount;
    private PlayerController _playercontroller;
    private Vector3 _camDeffaultPos;
    private float _cameraSwingOffset;
    private bool _isMoving;

    void Start()
    {
        _playercontroller = FindObjectOfType<PlayerController>();
        _cameraSwingOffset = 0f; // Инициализация смещения покачивания камеры
        _camDeffaultPos = new Vector3(transform.localPosition.y, transform.localPosition.y, transform.localPosition.y);
    }

    void Update() => CameraSwing();
    

    private void CameraSwing()
    {
        // Проверяем, если персонаж движется, чтобы активировать покачивание камеры
        _isMoving = Mathf.Abs(_playercontroller.VerticalInput) > 0f || Mathf.Abs(_playercontroller.HorizontalInput) > 0f;

        // Рассчитываем смещение покачивания камеры при передвижении
        if (_isMoving)
        {
            _cameraSwingOffset = Mathf.Sin(Time.time * (_cameraSwingMultiplier * _playercontroller.CharacterSpeed)) * _cameraSwingAmount;
        }
        else
        {
            // Плавное затухание покачивания камеры при остановке персонажа
            _cameraSwingOffset = Mathf.Lerp(_cameraSwingOffset, 0f, Time.deltaTime * (_cameraSwingMultiplier * _playercontroller.CharacterSpeed));
        }
        Debug.Log(_cameraSwingOffset);
        transform.localPosition = new Vector3(0, (_camDeffaultPos.y + _cameraSwingOffset / 10), 0);
    }
}

