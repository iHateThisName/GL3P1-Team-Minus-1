using UnityEngine;
using UnityEngine.InputSystem;
using Xasu.Util;

public class GodMode : Singleton<GodMode> {
    [SerializeField]
    private InputActionReference modeActivatedAction;

    [SerializeField] private InputActionReference cheatActionRefrence;
    [SerializeField] private InputActionReference disableBreating;
    [SerializeField] private InputActionReference unlockAllCheckPoints;
    [SerializeField] private InputActionReference teleportPlayerToTheBottom;
    [SerializeField] private InputActionReference giveMoney;
    [SerializeField] private InputActionReference teleportPlayerToTheTop;


    private bool isHoldingDown = false;

    [SerializeField] private Transform cheatMenu;

    protected override void Awake() {
        base.Awake();
        this.cheatMenu.gameObject.SetActive(false);
    }
    void OnEnable() {
        // Enable the action when the script is enabled
        this.cheatActionRefrence.action.Enable();
        this.disableBreating.action.Enable();
        this.unlockAllCheckPoints.action.Enable();
        this.teleportPlayerToTheBottom.action.Enable();
        this.giveMoney.action.Enable();
        this.teleportPlayerToTheTop.action.Enable();



        this.modeActivatedAction.action.performed += OnGodStarted;
        this.cheatActionRefrence.action.performed += OnHoldingDown;
        this.cheatActionRefrence.action.canceled += OnReleased;
        this.disableBreating.action.performed += OnDisableBreathingMechanic;
        this.unlockAllCheckPoints.action.performed += OnUnlockAllCheckpoints;
        this.teleportPlayerToTheBottom.action.performed += OnTeleportPlayerToTheBottom;
        this.teleportPlayerToTheTop.action.performed += OnTeleportPlayerToTheTop;
        this.giveMoney.action.performed += OnGiveMoney;
    }


    private void OnDisable() {
        // Disable the action when the script is disabled
        this.cheatActionRefrence.action.Disable();
        this.disableBreating.action.Disable();
        this.unlockAllCheckPoints.action.Disable();
        this.teleportPlayerToTheBottom.action.Disable();
        this.teleportPlayerToTheTop.action.Disable();
        this.giveMoney.action.Disable();

        this.modeActivatedAction.action.performed -= OnGodStarted;
        this.cheatActionRefrence.action.performed -= OnHoldingDown;
        this.cheatActionRefrence.action.canceled -= OnReleased;

        this.disableBreating.action.performed -= OnDisableBreathingMechanic;
        this.unlockAllCheckPoints.action.performed -= OnUnlockAllCheckpoints;
        this.giveMoney.action.performed -= OnGiveMoney;

        this.teleportPlayerToTheTop.action.performed -= OnTeleportPlayerToTheTop;
        this.teleportPlayerToTheBottom.action.performed -= OnTeleportPlayerToTheBottom;
    }

    private void OnHoldingDown(InputAction.CallbackContext context) {
        this.isHoldingDown = true;
        this.cheatMenu.gameObject.SetActive(true);
    }
    private void OnReleased(InputAction.CallbackContext context) {
        this.isHoldingDown = false;
        this.cheatMenu.gameObject.SetActive(false);
    }
    private void OnDisableBreathingMechanic(InputAction.CallbackContext context) {
        if (!this.isHoldingDown) return;

        GameManager.Instance.BreathingScript.DisableBreathing();
        GameManager.Instance.BreathingScript.enabled = false;

        GameManager.Instance.TutorialScript.OnSkippedTut(new InputAction.CallbackContext());
    }

    private void OnGiveMoney(InputAction.CallbackContext context) {
        if (!this.isHoldingDown) return;
        GameManager.Instance.Money += 10000;
    }

    private void OnUnlockAllCheckpoints(InputAction.CallbackContext context) {
        if (!this.isHoldingDown) return;
        CheckPointManager.Instance.UnlockAllCheckpoints();
    }

    private void OnTeleportPlayerToTheBottom(InputAction.CallbackContext context) {
        if (!this.isHoldingDown) return;

        Vector3 bottomVector3 = new Vector3(308.31f, -899.87f, 0);
        GameManager.Instance.TeleportPlayer(bottomVector3);
    }

    private void OnTeleportPlayerToTheTop(InputAction.CallbackContext context) {
        if (!this.isHoldingDown) return;

        Vector3 topVector3 = new Vector3(-5f, 1f, 0);
        GameManager.Instance.TeleportPlayer(topVector3);
    }


    private void OnGodStarted(InputAction.CallbackContext context) {
        Debug.Log("This has been removed. GET GOOD");
        //GameManager.Instance.BreathingScript.DisableBreathing();
        //GameManager.Instance.BreathingScript.enabled = false;
        //CheckPointManager.Instance.UnlockAllCheckpoints();
        //GameManager.Instance.Money += 100000;
        //ScreenDialouge.Instance.speedUpFactor = 0f;
    }
}
