using UnityEngine;

public class ReturnOutOfMap : MonoBehaviour
{
    private Vector3 _defaultPos;
    private void Start() => _defaultPos = new Vector3(0, 1f, 0);

    private void Update()
    {
        if(transform.position.y < -70) transform.position = _defaultPos;
    }
}
