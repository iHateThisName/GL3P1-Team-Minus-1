using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BreathingScript : MonoBehaviour
{
    [Header("Breathing values")]
    [SerializeField]
    private float breathingBar = 0f;

    //The breathing should increase roughly by 12.5 per seconds.
    //This means that it should take about 4 seconds to reach 50
    [SerializeField]
    private float breathingSpeed = 12.5f;

    [SerializeField]
    private float minBreatheValue = 50f;

    [SerializeField]
    private float maxBreatheValue = 62.5f;

    private float holdingTimer = 0f;

    private float noAirTimer = 0f;

    private bool isBreathingIn = false;
    private bool isHoldingBreath = false;

    [SerializeField]
    private Scrollbar breathingSlider;

    private bool isBreathingOut => !isBreathingIn && !isHoldingBreath;

    [Header("Input Actions")]
    [SerializeField]
    private InputAction breatheInAction;
    [SerializeField]
    private InputAction holdBreathAction;

    private void OnEnable() {
        breatheInAction.Enable();
        holdBreathAction.Enable();

        breatheInAction.performed += OnBreatheInStarted;
        breatheInAction.canceled += OnBreatheInStopped;

        holdBreathAction.performed += OnHoldBreathStarted;
        holdBreathAction.canceled += OnHoldBreathStopped;
    }

    private void OnDisable() {
        breatheInAction.Disable();
        holdBreathAction.Disable();

        breatheInAction.performed -= OnBreatheInStarted;
        breatheInAction.canceled -= OnBreatheInStopped;

        holdBreathAction.performed -= OnHoldBreathStarted;
        holdBreathAction.canceled -= OnHoldBreathStopped;
    }

    // Update is called once per frame
    void Update() {
        breathingBar = Mathf.Clamp(breathingBar, 0f, 100f);
        Breathing();
        breathingSlider.value = breathingBar / 100f;
    }

    //Function used for the breathing logic
    private void Breathing() {
        if (isBreathingIn && !isHoldingBreath) {
            breathingBar += breathingSpeed * Time.deltaTime;
        }
        else if (isHoldingBreath) {
            holdingTimer += Time.deltaTime;
        }
        else if (isBreathingOut) {
            if(breathingBar <= 0f) {
                noAirTimer += Time.deltaTime;
            }
            else {
                breathingBar -= breathingSpeed * Time.deltaTime;
            }
        }
    }

    private void OnBreatheInStarted(InputAction.CallbackContext context) {
        isBreathingIn = true;
        if(breathingBar <= 4f && noAirTimer < 3f) {
            Debug.Log("Started breathing at correct time");
        }
        else {
            Debug.Log("Started breathing too early or too late");
        }
        noAirTimer = 0f;
    }
    private void OnBreatheInStopped(InputAction.CallbackContext context) {
        isBreathingIn = false;
    }
    private void OnHoldBreathStarted(InputAction.CallbackContext context) {
        isHoldingBreath = true;
        if(breathingBar >= minBreatheValue && breathingBar <= maxBreatheValue) {
            Debug.Log("Started holding at correct time");
        }
        else {
            Debug.Log("Started holding too early or too late");
        }
    }
    private void OnHoldBreathStopped(InputAction.CallbackContext context) {
        isHoldingBreath = false;
        if(holdingTimer >= 4f && holdingTimer <= 7f) {
            Debug.Log("Held breath for the correct amount of time");
        }
        else {
            Debug.Log("Held breath for too long or not long enough");
        }
        holdingTimer = 0;
    }

    public void DisableBreathing()
    {
        breathingBar = 0f;
        holdingTimer = 0f;
        noAirTimer = 0f;
        breathingSlider.value = 0f;
        breathingSlider.gameObject.SetActive(false);
    }
}
