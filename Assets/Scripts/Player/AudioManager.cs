using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Общая громкость")]
    [SerializeField, Tooltip("Общая громкость шагов")] private float _commonVolume;

    [Header("Звуки шагов")]
    [SerializeField, Tooltip("Стандартный звук шагов")] private AudioClip[] _steps;

    [Header("Громкость")]
    [SerializeField, Tooltip("Минимальная громкость шагов")] private float _minVolume;
    [SerializeField, Tooltip("Максимальная громкость шагов")] private float _maxVolume;

    [Header("Тональность")]
    [SerializeField, Tooltip("Минимальная тональность шагов")] private float _minPitch;
    [SerializeField, Tooltip("Максимальная тональность шагов")] private float _maxPitch;

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



