using UnityEngine;
using UnityEngine.InputSystem;

public class RopeScript : MonoBehaviour
{
    //The action for cutting rope
    [SerializeField]
    private InputActionReference cutAction;

    [SerializeField]
    private ConstantForce treasure;

    private bool inRange = false;

    private void OnEnable()
    {
        cutAction.action.performed += OnCutStarted;
    }

    private void OnDisable()
    {
        cutAction.action.performed -= OnCutStarted;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    private void OnCutStarted(InputAction.CallbackContext context)
    {
        if(inRange)
        {
            treasure.enabled = true;
            Destroy(this.gameObject);
        }
    }
}
