using UnityEngine;
using UnityEngine.InputSystem;

public class GodMode : MonoBehaviour
{
    [SerializeField]
    private InputAction modeActivatedAction;

    void OnEnable()
    {
        modeActivatedAction.Enable();

        modeActivatedAction.performed += OnGodStarted;
    }

    private void OnDisable()
    {
        modeActivatedAction.Disable();

        modeActivatedAction.canceled -= OnGodStarted;
    }

    void OnGodStarted(InputAction.CallbackContext context)
    {
        GameManager.Instance.BreathingScript.DisableBreathing();

        GameManager.Instance.BreathingScript.enabled = false;
    }
}
