using UnityEngine;
using UnityEngine.InputSystem;

public class RopeScript : MonoBehaviour
{
    //The action for cutting rope
    [SerializeField]
    private InputAction cutAction;

    [SerializeField]
    private ConstantForce treasure;

    private bool inRange = false;

    private void OnEnable()
    {
        cutAction.Enable();

        cutAction.performed += OnCutStarted;
        cutAction.canceled += OnCutStopped;
    }

    private void OnDisable()
    {
        cutAction.Disable();

        cutAction.performed -= OnCutStarted;
        cutAction.canceled -= OnCutStopped;
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

    private void OnCutStopped(InputAction.CallbackContext context)
    {
        if (inRange)
        {
            Debug.Log("Cut action finished");
        }
    }
}
