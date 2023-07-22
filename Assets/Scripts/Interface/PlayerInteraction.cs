using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _interactionUI;
    [SerializeField] private TextMeshProUGUI _interactionText;
    [SerializeField] private float _interactionDistance;

    private bool _rayValue;
    public bool RayValue { get => _rayValue; }

    void Update() => InteractionRay();
    
    void InteractionRay()
    {
        Ray _ray = _camera.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;

        bool hitSomething = false;
        _rayValue = Physics.Raycast(_ray, out hit, _interactionDistance);
        if (_rayValue == true)
        {

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {

                hitSomething = true;
                _interactionText.text = interactable.GetDescription();

                /*if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("InteractionRay");

                    interactable.Interact();
                }*/
            }
        }
        _interactionUI.SetActive(hitSomething);
    } 

}
