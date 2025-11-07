using UnityEngine;
using System.Collections.Generic;

public class DialougeLookUpManager : MonoBehaviour
{
    [SerializeField]
    private List<string[]> storyDialougeList = new List<string[]>();
    [SerializeField]
    private List<string[]> genericDialougeScript = new List<string[]>();

    private bool startedGame = true;
    private bool startedGameFinished;
    private bool firstTreasureCollected;
    private bool firstTreasureFinished;

    public string[] GetDialouge()
    {
        if (!startedGameFinished && startedGame)
        {
            return storyDialougeList[0];
        }
        else
        {
            return genericDialougeScript[0];
        }
    }
}
