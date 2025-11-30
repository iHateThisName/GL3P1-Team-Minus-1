using UnityEngine;
using TMPro;
using System.Collections;

public class SecretDialouge : MonoBehaviour
{
    [SerializeField]
    private TMP_Text playerText;

    [SerializeField]
    private string secretMessage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(!GameManager.Instance.secretDialougeTriggered)
            {
                GameManager.Instance.secretDialougeTriggered = true;
                int randomNum = Random.Range(0, 101);
                if (randomNum <= 5)
                {
                    GameManager.Instance.secretDialougeActive = true;
                    StartCoroutine(DisplaySecretMessage());
                }
            }
        }
    }

    IEnumerator DisplaySecretMessage()
    {
        playerText.text = secretMessage;
        yield return new WaitForSeconds(5);
        playerText.text = "";
    }
}
