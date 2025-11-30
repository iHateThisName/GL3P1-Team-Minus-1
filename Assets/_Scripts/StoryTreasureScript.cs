using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

    //The action for closing the screen
    [SerializeField]
    private InputActionReference closeScreenAction;

    private bool treasureOpen;

    private void OnEnable()
    {
        closeScreenAction.action.performed += CloseScreen;
    }

    private void OnDisable()
    {
        closeScreenAction.action.performed -= CloseScreen;
    }

    public void DisplayTreasureScreen(int treasureNum, string treasureMessage, bool isStory)
    {
        currentScreen = treasureNum;
        currentMessage = treasureMessage;
        storyBool = isStory;
        fadeScreen.SetActive(true);
        treasureScreens[currentScreen].SetActive(true);
        treasureOpen = true;
    }

    public void CloseScreen(InputAction.CallbackContext context)
    {
        if(treasureOpen == true)
        {
            treasureScreens[currentScreen].SetActive(false);
            fadeScreen.SetActive(false);
            GameManager.Instance.PlayerMovement.enabled = true;
            breathingScript.enabled = true;
            treasureOpen = false;

            if (storyBool)
            {
                StartCoroutine(DisplayStoryMessage());
            }
        }
    }

    IEnumerator DisplayStoryMessage()
    {
        playerText.text = currentMessage;
        yield return new WaitForSeconds(5);
        playerText.text = "";
    }
}
