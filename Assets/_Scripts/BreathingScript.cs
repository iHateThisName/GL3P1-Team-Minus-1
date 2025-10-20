using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private TMP_Text holdingText;

    [SerializeField]
    private TMP_Text noAirText;

    [SerializeField]
    private RawImage correctImage;

    private float inhaleTimer;


    [Header("Oxygen Values")]
    public float oxygenAmount = 1000f;

    public float intendedOxygen = 1000f;

    [SerializeField]
    private float oxygenLoss = 10f;

    private float oxygenPunishment = 0f;

    public Slider oxygenSlider;

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
        if (this.breathingSlider == null) return;

        breathingBar = Mathf.Clamp(breathingBar, 0f, 100f);
        oxygenAmount = Mathf.Clamp(oxygenAmount, 0f, intendedOxygen);
        Breathing();
        breathingSlider.value = breathingBar / 100f;

        oxygenSlider.value = oxygenAmount;

        if(oxygenAmount <= 0f)
        {
            Die();
        }
    }

    //Function used for the breathing logic
    private void Breathing() {
        if (isBreathingIn && !isHoldingBreath) {
            breathingBar += breathingSpeed * Time.deltaTime;
            inhaleTimer += Time.deltaTime;
            if(inhaleTimer >= 10f)
            {
                Die();
            }
        }
        else if (isHoldingBreath) {
            holdingTimer += Time.deltaTime;
            holdingText.text = holdingTimer.ToString("F2");
            if(holdingTimer >= 10f)
            {
                Die();
            }
        }
        else if (isBreathingOut) {
            if(breathingBar <= 0f) {
                noAirTimer += Time.deltaTime;
                noAirText.text = noAirTimer.ToString("F2");
                if(noAirTimer >= 10f)
                {
                    Die();
                }
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
            correctImage.color = Color.green;
        }
        else {
            Debug.Log("Started breathing too early or too late");
            oxygenPunishment += 5f;
            correctImage.color = Color.red;
        }
        noAirTimer = 0f;
        holdingText.text = "";
        noAirText.text = "";
    }
    private void OnBreatheInStopped(InputAction.CallbackContext context) {
        isBreathingIn = false;
        if (breathingBar > maxBreatheValue || breathingBar < minBreatheValue)
        {
            Debug.Log("Inhaled for too long or too short");
            oxygenPunishment += 5f;
            correctImage.color = Color.red;

            if(!isHoldingBreath)
            {
                oxygenAmount -= oxygenLoss + oxygenPunishment;
                oxygenPunishment = 0f;
            }
        }
        inhaleTimer = 0f;
    }
    private void OnHoldBreathStarted(InputAction.CallbackContext context) {
        isHoldingBreath = true;
        if(breathingBar >= minBreatheValue && breathingBar <= maxBreatheValue) {
            Debug.Log("Started holding at correct time");
            correctImage.color = Color.green;
        }
        else {
            Debug.Log("Started holding too early or too late");
            oxygenPunishment += 5f;
            correctImage.color = Color.red;
        }
    }
    private void OnHoldBreathStopped(InputAction.CallbackContext context) {
        isHoldingBreath = false;
        if(holdingTimer >= 4f && holdingTimer <= 7f) {
            Debug.Log("Held breath for the correct amount of time");
            correctImage.color = Color.green;
        }
        else {
            Debug.Log("Held breath for too long or not long enough");
            oxygenPunishment += 5f;
            correctImage.color = Color.red;
        }
        oxygenAmount -= oxygenLoss + oxygenPunishment;
        oxygenPunishment = 0f;
        holdingTimer = 0;
        holdingText.text = "";
    }

    public void DisableBreathing()
    {
        oxygenAmount = intendedOxygen;
        oxygenPunishment = 0f;
        breathingBar = 0f;
        holdingTimer = 0f;
        noAirTimer = 0f;
        breathingSlider.value = 0f;
        holdingText.text = "";
        noAirText.text = "";
    }

    private void Die()
    {
        SceneManager.LoadScene("GameOver");
    }
}
