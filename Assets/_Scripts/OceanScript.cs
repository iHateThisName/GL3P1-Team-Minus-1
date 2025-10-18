using UnityEngine;

public class OceanScript : MonoBehaviour {

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private BreathingScript breatheScript;
    [SerializeField] private GameObject breatheSlider;
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            this.playerMovement.isUnderWater = true;
            this.breatheScript.enabled = true;
            breatheSlider.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            this.playerMovement.isUnderWater = false;
            this.breatheScript.DisableBreathing();
            this.breatheScript.enabled = false;
            breatheSlider.SetActive(false);
        }
    }
}
