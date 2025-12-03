using UnityEngine;

public class SlowDown : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.SlowDownPlayer();
        }
    }
}
