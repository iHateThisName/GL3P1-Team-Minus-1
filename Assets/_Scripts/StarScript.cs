using UnityEngine;

public class StarScript : MonoBehaviour {

    private bool isTriggered = false;
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !this.isTriggered) {
            this.isTriggered = true;
            Debug.Log("Player touched the star!");
            Trigger();

        }
    }

    [ContextMenu("Trigger End")]
    private void Trigger() {
        GameManager.Instance.endingTriggered = true;
        TransitionController.Instance.RollCredits();
    }
}
