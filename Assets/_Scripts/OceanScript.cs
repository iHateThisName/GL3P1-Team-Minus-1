using UnityEngine;

public class OceanScript : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            GameManager.Instance.PlayerEnterOcean();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            GameManager.Instance.PlayerExitOcean();
        }
    }
}
