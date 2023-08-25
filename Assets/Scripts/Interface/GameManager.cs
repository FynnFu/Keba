using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    private PlayerControls _playerControls;
    private bool _isPauseActive;

    private void Awake() => _playerControls = new PlayerControls();
    private void OnEnable() => _playerControls.Inteface.Enable();
    private void OnDisable() => _playerControls.Inteface.Disable();

    private void Start()
    {
        _menu.SetActive(false);
        _isPauseActive = false;
    }

    private void Update()
    {
        if (_playerControls.Inteface.Menu.WasPressedThisFrame() && _isPauseActive == false)
        {
            Pause(true);
        }
        else if (_playerControls.Inteface.Menu.WasPressedThisFrame() && _isPauseActive == true)
        {
            Pause(false);
        }
    }

    private void Pause(bool pauseState)
    {
        Time.timeScale = pauseState ? 0f : 1.0f;
        _menu.SetActive(pauseState);
        _isPauseActive = pauseState;
    }

}
