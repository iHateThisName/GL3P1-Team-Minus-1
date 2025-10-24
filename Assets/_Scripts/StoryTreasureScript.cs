using UnityEngine;
using TMPro;
using System.Collections;

public class StoryTreasureScript : MonoBehaviour
{
    [SerializeField]
    private GameObject fadeScreen;

    [SerializeField]
    private GameObject[] treasureScreens;

    [SerializeField]
    private TMP_Text playerText;

    private int currentScreen;

    private string currentMessage;

    private bool storyBool;

    [SerializeField]
    private BreathingScript breathingScript;

    public void DisplayTreasureScreen(int treasureNum, string treasureMessage, bool isStory)
    {
        currentScreen = treasureNum;
        currentMessage = treasureMessage;
        storyBool = isStory;
        fadeScreen.SetActive(true);
        treasureScreens[currentScreen].SetActive(true);
    }

    public void CloseScreen()
    {
        treasureScreens[currentScreen].SetActive(false);
        fadeScreen.SetActive(false);
        GameManager.Instance.PlayerMovement.enabled = true;
        breathingScript.enabled = true;

        if(storyBool)
        {
            StartCoroutine(DisplayStoryMessage());
        }
    }

    IEnumerator DisplayStoryMessage()
    {
        playerText.text = currentMessage;
        yield return new WaitForSeconds(5);
        playerText.text = "";
    }
}
