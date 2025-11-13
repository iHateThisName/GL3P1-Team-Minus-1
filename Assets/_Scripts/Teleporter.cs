using UnityEngine;

public class Teleporter : Interactable {

    [SerializeField] private string text = "Teleporter Area";
    private bool isPlayerInRange = false;

    public override int GetValueAmount() => 0;
    public override void Interact() {
        if (!this.isPlayerInRange) return;
        CheckPointManager.Instance.SetCurrentCheckPoint(CheckPointManager.EnumCheckPoint.Store);
        CheckPointManager.Instance.UseCheckpoint();

    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Player entered teleporter area.");
            this.isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Player exited teleporter area.");
            this.isPlayerInRange = false;
        }
    }
}
