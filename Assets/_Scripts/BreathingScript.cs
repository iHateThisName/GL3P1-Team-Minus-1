using UnityEngine;
using UnityEngine.InputSystem;

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

    private bool isBreathingIn = false;
    private bool isHoldingBreath = false;

    private bool isBreathingOut => !isBreathingIn && !isHoldingBreath;

    [Header("Input Actions")]
    [SerializeField]
    private InputAction breatheInAction;
    [SerializeField]
    private InputAction holdBreathAction;

    private void OnEnable()
    {
        breatheInAction.Enable();
        holdBreathAction.Enable();

        breatheInAction.performed += OnBreatheInStarted;
        breatheInAction.canceled += OnBreatheInStopped;

        holdBreathAction.performed += OnHoldBreathStarted;
        holdBreathAction.canceled += OnHoldBreathStopped;
    }

    private void OnDisable()
    {
        breatheInAction.Disable();
        holdBreathAction.Disable();

        breatheInAction.performed -= OnBreatheInStarted;
        breatheInAction.canceled -= OnBreatheInStopped;

        holdBreathAction.performed -= OnHoldBreathStarted;
        holdBreathAction.canceled -= OnHoldBreathStopped;
    }

    // Update is called once per frame
    void Update()
    {
        breathingBar = Mathf.Clamp(breathingBar, 0f, 100f);
        Breathing();
    }

    //Function used for the breathing logic
    private void Breathing()
    {
        if (isBreathingIn && !isHoldingBreath)
        {
            Debug.Log("Breathing In");
            breathingBar += breathingSpeed * Time.deltaTime;
        }
        else if (isHoldingBreath)
        {
            Debug.Log("Holding breath");
        }
        else if (isBreathingOut)
        {
            Debug.Log("Breathing out");
            breathingBar -= breathingSpeed * Time.deltaTime;
        }
    }

    private void OnBreatheInStarted(InputAction.CallbackContext context)
    {
        isBreathingIn = true;
    }
    private void OnBreatheInStopped(InputAction.CallbackContext context)
    {
        isBreathingIn = false;
    }
    private void OnHoldBreathStarted(InputAction.CallbackContext context)
    {
        isHoldingBreath = true;
    }
    private void OnHoldBreathStopped(InputAction.CallbackContext context)
    {
        isHoldingBreath = false;
    }
}
