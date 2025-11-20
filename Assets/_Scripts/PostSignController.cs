using UnityEngine;
using UnityEngine.UI;

public class PostSignController : Interactable {

    [SerializeField] private Image promtImage;
    private bool isPlayerInRange = false;

    public override int GetValueAmount() => 0;

    public override void Interact() {
        if (!this.isPlayerInRange) return;
        // TODO Open a dialog window / zoom in on signpost??

        Debug.Log("Interacted with Post Sign.");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            this.isPlayerInRange = true;

            // shortly fade in the prompt
            StartCoroutine(TransitionController.Instance.FadeImageInCoroutine(this.promtImage, 0.5f));
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            this.isPlayerInRange = false;

            // shortly fade out the prompt
            StartCoroutine(TransitionController.Instance.FadeImageOutCoroutine(this.promtImage, 0.5f));
        }
    }
}
