using UnityEngine;

public class TurnOnLight : MonoBehaviour {
    [SerializeField]
    private bool turnOn;

    [SerializeField]
    private GameObject flashLight;
    [SerializeField]
    private GameObject emmisionBands;

    [SerializeField]
    private Material emissiveMat;
    [SerializeField]
    private Material normalMat;

    private void Start() {
        GameManager.Instance.TurnOnLight = this;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (turnOn) {
                TurnOn();
            } else {
                TurnOff();
            }
        }
    }

    public void TurnOn() {
        flashLight.SetActive(true);
        emmisionBands.GetComponent<Renderer>().material = emissiveMat;
    }

    public void TurnOff() {
        flashLight.SetActive(false);
        emmisionBands.GetComponent<Renderer>().material = normalMat;
    }
}
