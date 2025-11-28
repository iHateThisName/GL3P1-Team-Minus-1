using UnityEngine;

public class TurnOnLight : MonoBehaviour
{
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(turnOn)
            {
                flashLight.SetActive(true);
                emmisionBands.GetComponent<Renderer>().material = emissiveMat;
            }
            else
            {
                flashLight.SetActive(false);
                emmisionBands.GetComponent<Renderer>().material = normalMat;
            }
        }
    }
}
