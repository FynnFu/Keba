using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Tooltip("Стандартный звук шагов")][SerializeField] private AudioClip _stepDefault;
    private PlayerCam _playerCam;
    private AudioSource _audioSource;
    private float _delay;
    void Start()
    {
        _playerCam = FindAnyObjectByType<PlayerCam>();
        _audioSource = GetComponent<AudioSource>();
        InvokeRepeating("PlayStep", 0f, _stepDefault.length);
    }

    void FixedUpdate()
    {

    }

    private void PlayStep()
    {
        /*if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)  && _playerCam.SinusTimerY >= -_playerCam.СamSwingAmount)
        {
            _audioSource.PlayOneShot(_stepDefault);
        }*/
    }
}
