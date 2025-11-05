using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BreathingScript : MonoBehaviour
{
    [Header("Breathing values")]
    //The bar that rises up and down depending on your breathing
    [SerializeField]
    private float breathingBar = 0f;

    public float sprintMultiplier = 0f;

    public float weightValue;
    public float upgradeValue = 1f;

    //The speed at which the breathing bar rises up
    [SerializeField]
    private float breathingSpeed = 12.5f;

    //The minimum value of the green zone of the breathing bar
    [SerializeField]
    private float minBreatheValue = 50f;

    //The max value of the green zone of the breathing bar
    [SerializeField]
    private float maxBreatheValue = 62.5f;

    //The diffrence between the max and min breathing values. Used to calculate perfect breathing
    [SerializeField]
    private float breathValueDiffrence;

    //The timer used to measure how long you have held your breath
    private float holdingTimer = 0f;

    //The timer used to measure how long you have been without oxygen
    private float noAirTimer = 0f;

    //Bool for checking if you are breathing in
    private bool isBreathingIn = false;
    //Bool for checking if you are holding your breath
    private bool isHoldingBreath = false;
    //A bool to check if you are breathing out
    private bool isBreathingOut => !isBreathingIn && !isHoldingBreath;
    //A bool for checking if you have held your breath this "turn". Used to punish those who avoid holding their breath
    private bool hasHeldBreath = true;

    //The slider used in the visualisation of your breathing
    [SerializeField]
    private Scrollbar breathingSlider;

    //The text used to communicate how long the player has held their breath
    [SerializeField]
    private TMP_Text holdingText;

    //The text used to communicate with the player how long they have been without air
    [SerializeField]
    private TMP_Text noAirText;

    //The image used to show if the player is breathing correctly 
    [SerializeField]
    private RawImage correctImage;

    //The timer used to check how long the player has been inhaling
    private float inhaleTimer;


    [Header("Oxygen Values")]
    //The amount of oxygen the player currently has
    public float oxygenAmount = 1000f;

    //The max oxygen that the player's tank has
    public float intendedOxygen = 1000f;

    //The base oxygen loss that the player experinces
    [SerializeField]
    private float oxygenLoss = 10f;

    //The punishment value added to oxygen amount if the player is breathing incorrectly
    private float oxygenPunishment = 0f;

    //The slider used to communicate how much oxygen the player has left
    public Slider oxygenSlider;

    [Header("Input Actions")]
    //The action for breathing in
    [SerializeField]
    private InputAction breatheInAction;
    //The action for breathing out
    [SerializeField]
    private InputAction holdBreathAction;

    private void Awake()
    {
        GameManager.Instance.BreathingScript = this;
    }

    //Enables and subscribes input actions when this script is enabled
    private void OnEnable() {
        breatheInAction.Enable();
        holdBreathAction.Enable();

        breatheInAction.performed += OnBreatheInStarted;
        breatheInAction.canceled += OnBreatheInStopped;

        holdBreathAction.performed += OnHoldBreathStarted;
        holdBreathAction.canceled += OnHoldBreathStopped;
    }

    //Disables and unsubscribes input actions when this script is disabled
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
        //Failsafe
        if (this.breathingSlider == null) return;

        //Clamps the breathing bar between 0 and 100
        breathingBar = Mathf.Clamp(breathingBar, 0f, 100f);
        //Clamps the oxygen amount between 0 and the maximum amount
        oxygenAmount = Mathf.Clamp(oxygenAmount, 0f, intendedOxygen);
        //Method used for breathing logic
        Breathing();
        //Sets the breathing slider to be equal to the breathing bar (divided by 100 because of limit)
        breathingSlider.value = breathingBar / 100f;
        //Sets the oxygen slider to be equal to the amount of oxygen the player has
        oxygenSlider.value = oxygenAmount;

        //If you run out of oxygen, you die :)
        if(oxygenAmount <= 0f)
        {
            Die();
        }
    }

    //Function used for the breathing logic
    private void Breathing() {
        //If you're breathing in, the breathing bar goes up
        if (isBreathingIn && !isHoldingBreath) {
            breathingBar += (breathingSpeed + sprintMultiplier + (weightValue * upgradeValue)) * Time.deltaTime;
            inhaleTimer += Time.deltaTime;
            //If you breathe in for too long, you die
            if(inhaleTimer >= 12f)
            {
                Die();
            }
        }
        //If you hold your breath, the breathing bar stays in place and a the timer for holding starts
        else if (isHoldingBreath) {
            holdingTimer += Time.deltaTime;
            holdingText.text = holdingTimer.ToString("F2");
            //If you hold your breath for too long, you die
            if(holdingTimer >= 15f)
            {
                Die();
            }
        }
        //If you're breathing out, the breathing bar goes down
        else if (isBreathingOut) {
            //If you have already reached the bottom, the breathing bar stays in place and the the no air timer is displayed
            if(breathingBar <= 0f) {
                noAirTimer += Time.deltaTime;
                noAirText.text = noAirTimer.ToString("F2");
                //If you don't breathe in for too long, you die
                if(noAirTimer >= 10f)
                {
                    Die();
                }
            }
            //If the bar hasn't reached the bottom, the bar goes down
            else {
                breathingBar -= (breathingSpeed + sprintMultiplier + (weightValue * upgradeValue)) * Time.deltaTime;
            }
        }
    }

    //Action for starting your breathing
    private void OnBreatheInStarted(InputAction.CallbackContext context) {
        isBreathingIn = true;
        //Check used to punish those who didn't hold their breath
        if(hasHeldBreath)
        {
            hasHeldBreath = false;
        }
        else
        {
            oxygenAmount -= oxygenLoss + oxygenPunishment + 15f;
            oxygenPunishment = 0f;
            hasHeldBreath = false;
        }
        //If you start breathing perfectly, you gain no punishment
        if(breathingBar <= 4f && noAirTimer <= 0f)
        {
            Debug.Log("Started breathing at perfect time");
            correctImage.color = Color.darkGreen;
        }
        //If you start breathing correctly, but not perfectly, you will gain a small punishment
        else if (breathingBar <= 4f && noAirTimer <= 3f && noAirTimer > 0f)
        {
            Debug.Log("Started breathing at correct time");
            correctImage.color = Color.green;
            oxygenPunishment += 4f;
        }
        //If you start breathing at the wrong time, you lose quite a bit of air
        else
        {
            Debug.Log("Started breathing too early or too late");
            oxygenPunishment += 10f;
            correctImage.color = Color.red;
        }
        //Resets the no air timers
        noAirTimer = 0f;
        holdingText.text = "";
        noAirText.text = "";
    }
    //Action for stopping breathing in
    private void OnBreatheInStopped(InputAction.CallbackContext context) {
        isBreathingIn = false;
        //If you stop inhaling at the wrong time, you get punished
        if (breathingBar > maxBreatheValue || breathingBar < minBreatheValue)
        {
            Debug.Log("Inhaled for too long or too short");
            oxygenPunishment += 10f;
            correctImage.color = Color.red;

            //If you aren't holding your breath, you lose the oxygen you were meant to losse
            if(!isHoldingBreath)
            {
                oxygenAmount -= oxygenLoss + oxygenPunishment;
                oxygenPunishment = 0f;
            }
        }
        inhaleTimer = 0f;
    }
    //Action for starting to hold your breath
    private void OnHoldBreathStarted(InputAction.CallbackContext context) {
        isHoldingBreath = true;
        //Calculates the diffrence between the max and min oxygen values and halves them, used to check for perfect breathing
        breathValueDiffrence = (maxBreatheValue - minBreatheValue) * 0.5f;
        //If you start holding your breath at perfect time, you get no punishment
        if (breathingBar >= minBreatheValue && breathingBar <= minBreatheValue + breathValueDiffrence)
        {
            Debug.Log("Started holding at perfect time");
            correctImage.color = Color.darkGreen;
        }
        //If you start holding your breath at imperfect time, you gain a small punishment
        else if(breathingBar > minBreatheValue + breathValueDiffrence && breathingBar <= maxBreatheValue) {
            Debug.Log("Started holding at correct time");
            oxygenPunishment += 4f;
            correctImage.color = Color.green;
        }
        //If you start holding at incorrect time, you gain a big punishment
        else {
            Debug.Log("Started holding too early or too late");
            oxygenPunishment += 10f;
            correctImage.color = Color.red;
        }
        //Makes sure the game knows that the player held their breath
        hasHeldBreath = true;
    }
    //Action for stopping holding your breath
    private void OnHoldBreathStopped(InputAction.CallbackContext context) {
        isHoldingBreath = false;
        //If you stop holding your breath at perfect time, you gain no punishment
        if(holdingTimer >= 4f && holdingTimer < 5f)
        {
            Debug.Log("Held breath for the perfect amount of time");
            correctImage.color = Color.darkGreen;
        }
        //If you stop holding your breath at imperfect time, you gain a small punishment
        else if(holdingTimer >= 5f && holdingTimer <= 7f) {
            Debug.Log("Held breath for the correct amount of time");
            oxygenPunishment += 4f;
            correctImage.color = Color.green;
        }
        //If you stop holding at incorrect times, you gain a big punishment
        else {
            Debug.Log("Held breath for too long or not long enough");
            oxygenPunishment += 10f;
            correctImage.color = Color.red;
        }
        oxygenAmount -= oxygenLoss + oxygenPunishment;
        oxygenPunishment = 0f;
        holdingTimer = 0;
        holdingText.text = "";
    }

    //Method for resetting everything
    public void DisableBreathing()
    {
        oxygenAmount = intendedOxygen;
        oxygenPunishment = 0f;
        breathingBar = 0f;
        holdingTimer = 0f;
        noAirTimer = 0f;
        breathingSlider.value = 0f;
        sprintMultiplier = 0f;
        isBreathingIn = false;
        isHoldingBreath = false;
        hasHeldBreath = true;
        holdingText.text = "";
        noAirText.text = "";
    }

    //Method for dying
    private void Die()
    {
        SceneManager.LoadScene("GameOver");
    }

    /// <summary>
    /// Increases the player's weight by the specified amount
    /// </summary>
    /// <param name="amount">How much to increase by</param>
    public void IncreaseWeight(float amount)
    {
        this.weightValue += amount;
    }

    /// <summary>
    /// Decreases the player's weight by the specified amount
    /// </summary>
    /// <param name="amount">How much to decrease by</param>
    public void DecreaseWeight(float amount)
    {
        this.weightValue -= amount;
    }
}
