using System.Collections.Generic;
using UnityEngine;

public class OceanScript : MonoBehaviour {

    [SerializeField] private GameObject breatheSlider;

    [SerializeField] private BreathingScript breathingScript;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.PlayerMovement.isUnderWater = true;
            breathingScript.enabled = true;
            breatheSlider.SetActive(true);

            breathingScript.oxygenAmount = breathingScript.intendedOxygen;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.PlayerMovement.isUnderWater = false;
            breathingScript.DisableBreathing();
            breathingScript.enabled = false;
            breatheSlider.SetActive(false);
        }
    }
}
