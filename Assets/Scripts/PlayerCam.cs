using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private bool _isForward;
    private bool _isBack;
    private bool _isLeft;
    private bool _isRight;
    private float _shakeSpeed;
    private float _shake;
    private PlayerController _playerController;
    private void Start()
    {
        _playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        // IsInput();
        // CamShaking();
    }

    private void IsInput()
    {
        _isForward = Input.GetKey(KeyCode.W);
        _isBack = Input.GetKey(KeyCode.S);
    }

    private void CamShaking()
    {
        Debug.Log(transform.localPosition.y);

        if(_isForward || _isBack)
        {
            _shakeSpeed = 10f;
        } else _shakeSpeed = 0;
    }
}
