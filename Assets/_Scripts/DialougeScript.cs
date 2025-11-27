using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialougeScript : MonoBehaviour {
    [SerializeField]
    private InputAction dialougeStartAction;

    //A refrence to the dialouge text
    [SerializeField]
    private TMP_Text dialougeText;

    //An array of all the dialouge that will be displayed
    [SerializeField]
    private Dialog dialouge;

    //A bool to check if the player is in range of the object
    [SerializeField]
    private bool playerInRange = false;

    //The current line in the dialouge
    [SerializeField]
    private int lineNumber;

    //A bool for if the player is in dialouge
    private bool inDialouge;

    private bool useScreenDialouge = true;

    private void OnEnable() {
        dialougeStartAction.Enable();

        dialougeStartAction.performed += OnDialougeStarted;
    }

    private void OnDisable() {
        dialougeStartAction.Disable();

        dialougeStartAction.canceled -= OnDialougeStarted;
    }

    private void OnDialougeStarted(InputAction.CallbackContext context) {
        if (playerInRange) {
            if (!inDialouge) {
                GameManager.Instance.IsPlayerMovementEnabled = false;
                GameManager.Instance.PlayerMovement.ResetAnims();
                GetDialouge();
            } else {
                //If the player has not reached the end of the dialouge, the next line will be displayed
                if (lineNumber < dialouge.dialogLines.Count) {
                    ShowNextLine();
                }
                //If the player has reached the end of the dialouge, the dialouge will close
                else {
                    if (this.useScreenDialouge) {
                        ScreenDialouge.Instance.CloseDialougeScreen();
                    }

                    dialougeText.enabled = false;
                    inDialouge = false;
                    lineNumber = 0;
                    GameManager.Instance.IsPlayerMovementEnabled = true;

                }
            }
        }
    }

    //If the player enters the object's trigger, it will register that the player is in there and the outline will turn on if allowed
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerInRange = true;
        }
    }

    //If the player exits the object's trigger, it will register that the player is not there and the dialouge will turn off
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerInRange = false;
        }
    }

    private void GetDialouge() {
        dialouge = DialougeLookUpManager.Instance.GetDialouge(EnumCharacter.LifeGauard);
        inDialouge = true;
        if (this.useScreenDialouge) {
            ScreenDialouge.Instance.speedUpFactor = 1f;
        }
        ShowNextLine();
    }

    private void ShowNextLine() {
        if (this.useScreenDialouge) {
            if (ScreenDialouge.Instance.isPlayingDialogLine) {
                ScreenDialouge.Instance.speedUpFactor = 0.5f;
            } else {
                StartCoroutine(GlobalNextLine(dialouge.dialogLines[this.lineNumber]));
            }
        } else {
            NextLine();
        }
    }

    private IEnumerator GlobalNextLine(string line) {
        yield return StartCoroutine(ScreenDialouge.Instance.ShowDialougeScreen(line));
        this.lineNumber++;

    }

    //A function for displaying the next dialouge line
    private void NextLine() {
        dialougeText.enabled = true;
        dialougeText.text = this.dialouge.dialogLines[lineNumber];
        lineNumber++;
    }
}
