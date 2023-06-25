using System;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Tooltip("Скорость покачивания камеры")][Range(1f, 10f)][SerializeField] private float _cameraSwingMultiplier;
    [Tooltip("Дистанция покачивания")] [Range (0.01f, 1f)] [SerializeField] private float _cameraSwingAmount;
    private PlayerController _playerController;
    private PlayerInteraction _playerInteraction;
    private Vector3 _camDeffaultPos;
    private float _cameraSwingOffsetY;
    private float _cameraSwingOffsetX;
    private bool _isMoving;
    private const float _CamTimeDividerX = 2;

    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _playerInteraction = FindObjectOfType<PlayerInteraction>();
        _camDeffaultPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.y);
    }

    void Update() => CameraSwing();
    
    private void CameraSwing()
    {
        // Проверяем, если персонаж движется, чтобы активировать покачивание камеры
        _isMoving = Mathf.Abs(_playerController.VerticalInput) > 0f || Mathf.Abs(_playerController.HorizontalInput) > 0f;

        // Рассчитываем смещение покачивания камеры при передвижении
        if (_isMoving && _playerInteraction.RayValue == false)
        {
            _cameraSwingOffsetY = Mathf.Sin(Time.time * (_cameraSwingMultiplier * _playerController.CharacterSpeed) ) * _cameraSwingAmount;
            _cameraSwingOffsetX = Mathf.Sin(Time.time * (_cameraSwingMultiplier * _playerController.CharacterSpeed / _CamTimeDividerX)) * _cameraSwingAmount;
        }
        else
        {
            // Плавное затухание покачивания камеры при остановке персонажа
            _cameraSwingOffsetY = Mathf.Lerp(_cameraSwingOffsetY, 0f, Time.deltaTime * (_cameraSwingMultiplier * _playerController.CharacterSpeed));
            _cameraSwingOffsetX = Mathf.Lerp(_cameraSwingOffsetX, 0f, Time.deltaTime * (_cameraSwingMultiplier * _playerController.CharacterSpeed));
        }
        transform.localPosition = new Vector3(_camDeffaultPos.x + _cameraSwingOffsetX, _camDeffaultPos.y + _cameraSwingOffsetY, 0);
    }
}

