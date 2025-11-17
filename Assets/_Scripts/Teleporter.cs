using UnityEngine;

public class Teleporter : Interactable {

    [SerializeField] private string text = "Teleporter Area";
    [SerializeField] private TMPro.TMP_Text infoText;
    [SerializeField] private TMPro.TMP_Text promptText;
    private bool isPlayerInRange = false;

    public override int GetValueAmount() => 0;

    private void Start() {
        this.promptText.text = "Press 'E' to go to shore";
        this.infoText.text = this.text;
    }
    public override void Interact() {
        if (!this.isPlayerInRange) return;
        CheckPointManager.Instance.SetCurrentCheckPoint(0);
        CheckPointManager.Instance.UseCheckpoint();

    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            Debug.Log("Player entered teleporter area.");
            this.isPlayerInRange = true;

            // shortly fade in the prompt text
            StartCoroutine(TransitionController.Instance.FadeTextInCoroutine(this.promptText, 0.5f));
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            Debug.Log("Player exited teleporter area.");
            this.isPlayerInRange = false;

            // shortly fade out the prompt text
            StartCoroutine(TransitionController.Instance.FadeTextOutCoroutine(this.promptText, 0.5f));
        }
    }
}
