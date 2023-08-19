using System.Diagnostics.Tracing;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AudioManager : MonoBehaviour
{
    [Tooltip("Стандартный звук шагов")] [SerializeField] private AudioClip[] _steps;
    [Tooltip("Общая громкость шагов")] [SerializeField] private float _commonVolume;
    [Tooltip("Минимальная громкость шагов")] [SerializeField] private float _minVolume;
    [Tooltip("Максимальная громкость шагов")] [SerializeField] private float _maxVolume;
    [Tooltip("Минимальная тональность шагов")] [SerializeField] private float _minPitch;
    [Tooltip("Максимальная тональность шагов")] [SerializeField] private float _maxPitch;

    private PlayerCam _playerCam;
    private PlayerController _playerController;
    private AudioSource _audioSource;

    private int _counter;

    private const int _MaxCountVal = 2;

    void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _playerCam = FindAnyObjectByType<PlayerCam>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update() => PlayStep();


    private void PlayStep()
    {
        if (_playerCam.CamSwingOffsetY < 0 && _counter < _MaxCountVal) _counter++;
        else if (_playerCam.CamSwingOffsetY >= 0) _counter = 0;

        if (_counter == 1)
        {
            StepRandomizer();
            _audioSource.PlayOneShot(_steps[Random.Range(0, _steps.Length)]);
        }
    }

    private void StepRandomizer()
    {
        _audioSource.volume = Random.Range(_minVolume, _maxVolume) * _commonVolume;
        _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
    }
}



