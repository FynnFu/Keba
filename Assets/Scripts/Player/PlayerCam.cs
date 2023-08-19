using System;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Tooltip("Скорость покачивания камеры")] [Range(1f, 10f)] [SerializeField] private float _camSwingMultiplier;
    [Tooltip("Дистанция покачивания по вертикали")] [Range (0.01f, 1f)] [SerializeField] private float _camSwingAmountY;
    [Tooltip("Дистанция покачивания по горизонтали")] [Range(0.01f, 1f)] [SerializeField] private float _camSwingAmountX;
    [Tooltip("Позиция камеры в приседе")] [Range(0.1f, 1f)] [SerializeField] private float _squatOffset;
    [Tooltip("Скорость приседания")] [Range(1f, 30f)] [SerializeField] private float _squatSpeed;

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
    private const float _InputFallacy = 0.01f;

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
    }
    
    private void CameraSwing()
    {
        _camSwingSpeed = _camSwingMultiplier * _playerController.CharacterSpeed; // Скорость покачивания

        // Проверяем, если персонаж движется, чтобы активировать покачивание камеры
        _isMoving = Mathf.Abs(_playerController.VerticalInput) > _InputFallacy || Mathf.Abs(_playerController.HorizontalInput) > _InputFallacy;

        // Рассчитываем смещение покачивания камеры при передвижении
        _sinusTimerY = Mathf.Sin(Time.time * _camSwingSpeed) * _camSwingAmountY; // Позиция смещения по синусу для оси y
        _sinusTimerX = Mathf.Sin(Time.time * _camSwingSpeed / _CamTimeDividerX) * _camSwingAmountX; // Позиция смещения по синусу для оси x
        _camSwingOffsetY = SwingSmoother(_camSwingOffsetY, _isMoving && _playerInteraction.RayValue == false ? _sinusTimerY : 0f);
        _camSwingOffsetX = SwingSmoother(_camSwingOffsetX, _isMoving && _playerInteraction.RayValue == false ? _sinusTimerX : 0f);

        transform.localPosition = new Vector3(_camSwingOffsetX + _camPos.x, _camSwingOffsetY + _camPos.y, 0);
    }

    // Сглаживание покачивания
    private float SwingSmoother(float start, float end) => Mathf.Lerp(start, end, Time.deltaTime * (_camSwingMultiplier * _playerController.CharacterSpeed));

    // Смещение камеры по вертикали при приседании
    private void SquatOffset() 
    {
        _camTargetPos = (_playerController.IsSquat || _playerController.IsRayHit) ? _camDeffaultPos.y - _squatOffset : _camDeffaultPos.y;
        _camPos.y = Mathf.Lerp(_camPos.y, _camTargetPos, Time.deltaTime * _squatSpeed);
    } 


   
    /*transform.localPosition = _camDeffaultPos + UnityEngine.Random.insideUnitSphere * 0;*/
}

