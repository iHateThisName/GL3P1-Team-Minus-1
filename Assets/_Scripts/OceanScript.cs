using UnityEngine;

public class OceanScript : MonoBehaviour {

    [SerializeField] private PlayerMovement playerMovement;
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            this.playerMovement.SetWater(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            this.playerMovement.SetWater(false);
        }
    }
}
