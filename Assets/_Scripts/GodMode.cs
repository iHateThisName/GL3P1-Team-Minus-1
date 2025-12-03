using UnityEngine;
using UnityEngine.InputSystem;
using Xasu.Util;

public class GodMode : Singleton<GodMode> {
    [SerializeField]
    private InputActionReference modeActivatedAction;

    void OnEnable() {
        modeActivatedAction.action.performed += OnGodStarted;
    }

    private void OnDisable() {
        modeActivatedAction.action.performed -= OnGodStarted;
    }

    public void OnGodStarted(InputAction.CallbackContext context) {
        GameManager.Instance.BreathingScript.DisableBreathing();
        GameManager.Instance.BreathingScript.enabled = false;
        CheckPointManager.Instance.UnlockAllCheckpoints();
        GameManager.Instance.Money += 100000;
        ScreenDialouge.Instance.speedUpFactor = 0f;
    }
}
