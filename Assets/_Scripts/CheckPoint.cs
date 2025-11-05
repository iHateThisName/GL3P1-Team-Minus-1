using UnityEngine;

public class CheckPoint : MonoBehaviour {

    [SerializeField] private Transform teleportLocation;
    private void Start() {
        if (teleportLocation == null) {
            teleportLocation = this.transform;
        }
        CheckPointManager.Instance.RegisterCheckPoint(this);
    }

    public Transform GetTeleportLocation() => this.teleportLocation;
}
