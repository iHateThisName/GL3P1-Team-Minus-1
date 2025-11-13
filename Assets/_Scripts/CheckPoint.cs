using UnityEngine;

public class CheckPoint : MonoBehaviour {

    [SerializeField] private Transform teleportLocation;
    public CheckPointManager.EnumCheckPoint currentCheckPoint;
    private void Start() {
        if (teleportLocation == null) {
            teleportLocation = this.transform;
        }
        CheckPointManager.Instance.RegisterCheckPoint(this);
    }

    public Transform GetTeleportLocation() => this.teleportLocation;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            CheckPointManager.Instance.SetCurrentCheckPoint(this);
            Debug.Log($"Checkpoint {this.name} activated.");
            GameManager.Instance.PlayerExitOcean();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.PlayerEnterOcean();
            Debug.Log($"Player exited checkpoint {this.name}.");
        }
    }
}
