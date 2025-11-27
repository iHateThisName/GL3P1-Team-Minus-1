using UnityEngine;

public class OceanScript : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            if(!GameManager.Instance.firstEnteredOcean)
            {
                GameManager.Instance.PlayerMovement.ResetAnims();
                GameManager.Instance.PlayerMovement.rb.useGravity = false;
                GameSceneManager.Instance.LoadScene(EnumScene.Tutorial, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                GameManager.Instance.IsPlayerMovementEnabled = false;
            }
            else
            {
                GameManager.Instance.PlayerEnterOcean();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            GameManager.Instance.PlayerExitOcean();
        }
    }
}
