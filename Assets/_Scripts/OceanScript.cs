using System.Collections;
using UnityEngine;

public class OceanScript : MonoBehaviour {

    private bool isReady = true;

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
