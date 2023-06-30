using System.Diagnostics.Tracing;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AudioManager : MonoBehaviour
{
    [Tooltip("Стандартный звук шагов")][SerializeField] private AudioClip _stepDefault;
    private PlayerCam _playerCam;
    private AudioSource _audioSource;
    private float _minVolume = 0.5f;
    private float _maxVolume = 1f;
    private float _minPitch;
    private float _maxPitch;
    private int _counter;
    private const int _MaxCountVal = 2;
    void Start()
    {
        _playerCam = FindAnyObjectByType<PlayerCam>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        PlayStep();
    }

    private void PlayStep()
    {
        if (_playerCam.CamSwingOffsetY < 0 && _counter < _MaxCountVal) _counter++;
        else if (_playerCam.CamSwingOffsetY >= 0) _counter = 0;

        if (_counter == 1)
        {
            _audioSource.PlayOneShot(_stepDefault);
        }
    }



    /*private float StepRandomizer()
    {
        return 
    }*/

    /*private void PlayStep()
    {
        Debug.Log(_counter);
        if (_playerCam.CamSwingOffsetY < 0 && _counter < 2) _counter++;
        else if (_playerCam.CamSwingOffsetY >= 0) _counter = 0;
        if (_counter == 1) _audioSource.PlayOneShot(_stepDefault);
    }*/
}



