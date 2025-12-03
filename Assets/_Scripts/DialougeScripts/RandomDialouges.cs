using System.Collections;
using TMPro;
using UnityEngine;

public class RandomDialouges : MonoBehaviour {
    [SerializeField]
    private TMP_Text playerText;

    [SerializeField]
    private string randomMessage;

    private bool triggered;

    [SerializeField] private bool useFancyDialogue = false;

    [Tooltip("1 = 100% speed")]
    [SerializeField] private float fancyDialogueSpeedFactor = 1f;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !triggered) {
            StartCoroutine(DisplaySecretMessage());
        }
    }

    IEnumerator DisplaySecretMessage() {
        triggered = true;

        if (useFancyDialogue) {
            playerText.text = "";
            StartCoroutine(TransitionController.Instance.FadeTextInCoroutine(this.playerText, 1.25f));
            char previousChar = new char();
            foreach (char character in this.randomMessage.ToCharArray()) {

                if (character == '\\') {
                    Debug.Log("Found escape character");
                    previousChar = character;
                    continue; // Skip adding this

                } else if (previousChar == '\\' && character == 'n') {
                    playerText.text += "\n";
                    Debug.Log("Found new line character");

                } else {
                    playerText.text += character;
                }
                previousChar = character;
                yield return new WaitForSeconds(0.05f * this.fancyDialogueSpeedFactor);
            }
            // Add extra delay after finishing typing since fade in takes time
            yield return new WaitForSeconds(2.5f);
        } else {
            playerText.text = randomMessage;
        }
        yield return new WaitForSeconds(5);

        // Clear the text only if it hasn't been changed by a different dialogue
        if (this.playerText.text == randomMessage) {
            playerText.text = "";
        }
    }
}
