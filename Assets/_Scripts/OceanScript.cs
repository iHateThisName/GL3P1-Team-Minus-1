using UnityEngine;

public class OceanScript : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            if(!GameManager.Instance.firstEnteredOcean)
            {
                //GameSceneManager.Instance.
            }

            GameManager.Instance.PlayerEnterOcean();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            GameManager.Instance.PlayerExitOcean();
        }
    }
}
