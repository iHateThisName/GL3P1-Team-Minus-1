using UnityEngine;
using TMPro;
using System.Collections;

public class RandomDialouges : MonoBehaviour
{
    [SerializeField]
    private TMP_Text playerText;

    [SerializeField]
    private string randomMessage;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            StartCoroutine(DisplaySecretMessage());
        }
    }

    IEnumerator DisplaySecretMessage()
    {
        playerText.text = randomMessage;
        triggered = true;
        yield return new WaitForSeconds(5);
        playerText.text = "";
    }
}
