using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class TutorialScript : MonoBehaviour
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

    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference breatheInAction;
    [SerializeField]
    private InputActionReference holdBreathAction;
    [SerializeField]
    private InputActionReference interactAction;
    [SerializeField]
    private InputActionReference skipTutorialAction;

    [Header("Phase stuff")]
    [SerializeField]
    private int phaseNum;

    [SerializeField]
    private GameObject[] tutorialScreens;

    private int breatheInCorrect;
    private int holdBreathCorrect1;
    private int holdBreathCorrect2;
    private int breatheOutCorrect;

    private void OnEnable()
    {
        breatheInAction.action.performed += OnBreatheInStarted;
        breatheInAction.action.canceled += OnBreatheInStopped;

        holdBreathAction.action.performed += OnHoldBreathStarted;
        holdBreathAction.action.canceled += OnHoldBreathStopped;

        interactAction.action.performed += OnInteract;

        skipTutorialAction.action.performed += OnSkippedTut;
    }

    private void OnDisable()
    {
        breatheInAction.action.performed -= OnBreatheInStarted;
        breatheInAction.action.canceled -= OnBreatheInStopped;

        holdBreathAction.action.performed -= OnHoldBreathStarted;
        holdBreathAction.action.canceled -= OnHoldBreathStopped;

        interactAction.action.performed -= OnInteract;

        skipTutorialAction.action.performed -= OnSkippedTut;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.breathingSlider == null) return;

        breathingBar = Mathf.Clamp(breathingBar, 0f, 100f);
        Breathing();
        breathingSlider.value = breathingBar / 100f;
    }

    //Function used for the breathing logic
    private void Breathing() {
        if (isBreathingIn && !isHoldingBreath) {
            breathingBar += breathingSpeed * Time.deltaTime;
        } else if (isHoldingBreath && phaseNum > 0) {
            holdingTimer += Time.deltaTime;
            holdingText.text = holdingTimer.ToString("F2");
        } else if (isBreathingOut && phaseNum > 1) {
            if (breathingBar <= 0) {
                noAirTimer += Time.deltaTime;
                noAirText.text = noAirTimer.ToString("F2");
            } else {
                breathingBar -= breathingSpeed * Time.deltaTime;
            }
        }
    }

    private void OnBreatheInStarted(InputAction.CallbackContext context) {
        isBreathingIn = true;
        if (breathingBar <= 4f && noAirTimer < 3f) {
            Debug.Log("Started breathing at correct time");
            correctImage.color = Color.green;
            if (phaseNum == 2) {
                breatheOutCorrect++;
            }
        } else {
            Debug.Log("Started breathing too early or too late");
            correctImage.color = Color.red;
        }
        noAirTimer = 0f;
        if (phaseNum < 2) {
            breathingBar = 0f;
        } else if (phaseNum == 2 && breatheOutCorrect >= 3 && holdBreathCorrect1 >= 3 && holdBreathCorrect2 >= 3) {
            breatheOutCorrect = 0;
            breatheInCorrect = 0;
            holdBreathCorrect1 = 0;
            holdBreathCorrect2 = 0;
            NextPhase();
        }
        holdingText.text = "";
        noAirText.text = "";
    }
    private void OnBreatheInStopped(InputAction.CallbackContext context) {
        isBreathingIn = false;
        if (phaseNum == 0) {
            if (breathingBar >= minBreatheValue && breathingBar <= maxBreatheValue) {
                Debug.Log("Let go at correct time");
                correctImage.color = Color.green;
                breatheInCorrect++;
            } else {
                Debug.Log("Let go at incorrect time");
                correctImage.color = Color.red;
            }
            breathingBar = 0;
            if (breatheInCorrect >= 3) {
                breatheInCorrect = 0;
                NextPhase();
            }
        } else if (phaseNum == 1) {
            if (breathingBar >= minBreatheValue && breathingBar <= maxBreatheValue) {
                Debug.Log("Let go at correct time");
                correctImage.color = Color.green;
                breatheInCorrect++;
            } else {
                Debug.Log("Let go at incorrect time");
                correctImage.color = Color.red;
                breathingBar = 0;
            }
        } else {
            if (breathingBar > maxBreatheValue || breathingBar < minBreatheValue) {
                Debug.Log("Inhaled for too long");
                correctImage.color = Color.red;
            }
        }
    }
    private void OnHoldBreathStarted(InputAction.CallbackContext context) {
        isHoldingBreath = true;
        if (phaseNum > 0) {
            if (phaseNum == 1) {
                if (breathingBar >= minBreatheValue && breathingBar <= maxBreatheValue) {
                    Debug.Log("Started holding at correct time");
                    correctImage.color = Color.green;
                    holdBreathCorrect1++;
                } else {
                    Debug.Log("Started holding too early or too late");
                    correctImage.color = Color.red;
                    breathingBar = 0;
                }
            } else {
                if (breathingBar >= minBreatheValue && breathingBar <= maxBreatheValue) {
                    Debug.Log("Started holding at correct time");
                    correctImage.color = Color.green;
                    holdBreathCorrect1++;
                } else {
                    Debug.Log("Started holding too early or too late");
                    correctImage.color = Color.red;
                }
            }
        }
    }
    private void OnHoldBreathStopped(InputAction.CallbackContext context) {
        isHoldingBreath = false;
        if (phaseNum > 0) {
            if (phaseNum == 1) {
                if (holdingTimer >= 4f && holdingTimer <= 7f) {
                    Debug.Log("Held breath for the correct amount of time");
                    correctImage.color = Color.green;
                    holdBreathCorrect2++;
                } else {
                    Debug.Log("Held breath for too long or not long enough");
                    correctImage.color = Color.red;
                }
                breathingBar = 0;
                if (breatheInCorrect >= 3 && holdBreathCorrect1 >= 3 && holdBreathCorrect2 >= 3) {
                    breatheInCorrect = 0;
                    holdBreathCorrect1 = 0;
                    holdBreathCorrect2 = 0;
                    NextPhase();
                }
            } else {
                if (holdingTimer >= 4f && holdingTimer <= 7f) {
                    Debug.Log("Held breath for the correct amount of time");
                    correctImage.color = Color.green;
                    holdBreathCorrect2++;
                } else {
                    Debug.Log("Held breath for too long or not long enough");
                    correctImage.color = Color.red;
                }
            }
            holdingTimer = 0;
            holdingText.text = "";
        }
    }

    private void OnInteract(InputAction.CallbackContext context) {
        if (phaseNum > 2) {
            NextPhase();
        }
    }

    public void OnSkippedTut(InputAction.CallbackContext context) {
        GameSceneManager.Instance.UnloadeScene(EnumScene.Tutorial);
        GameManager.Instance.IsPlayerMovementEnabled = true;
        GameManager.Instance.firstEnteredOcean = true;
        GameManager.Instance.PlayerEnterOcean();
    }

    public void NextPhase() {
        if (phaseNum >= tutorialScreens.Length - 1) {
            GameSceneManager.Instance.UnloadeScene(EnumScene.Tutorial);
            GameManager.Instance.IsPlayerMovementEnabled = true;
            GameManager.Instance.firstEnteredOcean = true;
            GameManager.Instance.PlayerEnterOcean();
        } else {
            phaseNum++;
            tutorialScreens[phaseNum - 1].SetActive(false);
            tutorialScreens[phaseNum].SetActive(true);
        }
    }
}
