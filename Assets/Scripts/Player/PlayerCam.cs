using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Покаивание камеры")]
    [SerializeField, Tooltip("Скорость покачивания камеры"), Range(1f, 10f)] private float _camSwingMultiplier;
    [SerializeField, Tooltip("Дистанция покачивания по вертикали"), Range (0.01f, 1f)] private float _camSwingAmountY;
    [SerializeField, Tooltip("Дистанция покачивания по горизонтали"), Range(0.01f, 1f)] private float _camSwingAmountX;

    [Header("Смещение позиции камеры")]
    [SerializeField, Tooltip("Позиция камеры в приседе"), Range(0.1f, 1f)] private float _squatOffset;
    [SerializeField, Tooltip("Скорость приседания"), Range(1f, 30f)] private float _squatSpeed;

    [Header("Погрешность данных для устройств ввода")]
    [SerializeField, Range(0f, 0.03f)] private float _inputFallacy; // 0.016f +- for stick

    private PlayerController _playerController;
    private PlayerInteraction _playerInteraction;
    private Vector3 _camDeffaultPos;
    private Vector3 _camPos;

    private float _camSwingSpeed;
    private float _sinusTimerY;
    private float _sinusTimerX;
    private float _camSwingOffsetY;
    private float _camSwingOffsetX;
    private float _camTargetPos;
    private bool _isMoving;

    private const float _CamTimeDividerX = 2;

    public float CamSwingOffsetY { get => _camSwingOffsetY; }
    public float CamSwingAmountY { get => _camSwingAmountY; }

    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _playerInteraction = FindObjectOfType<PlayerInteraction>();
        _camDeffaultPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z); // Начальная позиция камеры
        _camPos = _camDeffaultPos;
    }

    void Update()
    {
        CameraSwing();
        SquatOffset();
        /*Debug.Log(_camSwingSpeed);*/
    }
    
    private void CameraSwing()
    {
        _camSwingSpeed = _camSwingMultiplier * _playerController.CharacterSpeed; // Скорость покачивания

        // Проверяем, если персонаж движется, чтобы активировать покачивание камеры
        _isMoving = Mathf.Abs(_playerController.VerticalInput) > _inputFallacy || Mathf.Abs(_playerController.HorizontalInput) > _inputFallacy;

        // Рассчитываем смещение покачивания камеры при передвижении
        _sinusTimerY = Mathf.Sin(Time.time * _camSwingSpeed) * _camSwingAmountY; // Позиция смещения по синусу для оси y
        _sinusTimerX = Mathf.Sin(Time.time * _camSwingSpeed / _CamTimeDividerX) * _camSwingAmountX; // Позиция смещения по синусу для оси x
        _camSwingOffsetY = SwingSmoother(_camSwingOffsetY, _isMoving && _playerInteraction.RayValue == false ? _sinusTimerY : 0f);
        _camSwingOffsetX = SwingSmoother(_camSwingOffsetX, _isMoving && _playerInteraction.RayValue == false ? _sinusTimerX : 0f);

        transform.localPosition = new Vector3(_camSwingOffsetX + _camPos.x, _camSwingOffsetY + _camPos.y, 0);
    }

    // Сглаживание покачивания
    private float SwingSmoother(float start, float end) => Mathf.Lerp(start, end, Time.deltaTime * _camSwingSpeed);

    // Смещение камеры по вертикали при приседании
    private void SquatOffset() 
    {
        _camTargetPos = (_playerController.IsSquat || _playerController.IsRayHit) ? _camDeffaultPos.y - _squatOffset : _camDeffaultPos.y;
        _camPos.y = Mathf.Lerp(_camPos.y, _camTargetPos, Time.deltaTime * _squatSpeed);
    } 


   
    /*transform.localPosition = _camDeffaultPos + UnityEngine.Random.insideUnitSphere * 0;*/
}

