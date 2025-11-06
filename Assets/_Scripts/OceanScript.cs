using UnityEngine;

public class OceanScript : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.PlayerEnterOcean();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.PlayerExitOcean();
        }
    }
}
