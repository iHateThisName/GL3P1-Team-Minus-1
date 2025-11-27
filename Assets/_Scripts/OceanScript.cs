using UnityEngine;

public class OceanScript : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            if(!GameManager.Instance.firstEnteredOcean)
            {
                GameSceneManager.Instance.LoadScene(EnumScene.Tutorial, UnityEngine.SceneManagement.LoadSceneMode.Additive);
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
