using Assets.Scripts.Singleton;
using System.Collections.Generic;
using UnityEngine;

public class DialougeLookUpManager : Singleton<DialougeLookUpManager>
{
    [SerializeField] private List<DialogTree> dialogTrees = new List<DialogTree>();
    private Dictionary<EnumCharacter, DialogTree> dialogTreesDict = new Dictionary<EnumCharacter, DialogTree>();

    [SerializeField] private bool isInDebugMode = false;

    protected override void Awake()
    {
        base.Awake();
        // Clear the dictionary to avoid stale data
        dialogTreesDict.Clear();

        // Populate the dictionary, skip duplicates and log a warning since there shuoldn't be any, but still allow the game to run
        foreach (DialogTree dialogTree in dialogTrees)
        {
            if (dialogTreesDict.ContainsKey(dialogTree.character))
            {
                Debug.LogWarning($"DialougeLookUpManager: Duplicate DialogTree for character {dialogTree.character} found. Skipping.");
                continue;
            }
            dialogTreesDict.Add(dialogTree.character, dialogTree);
        }
    }

    public Dialog GetDialouge(EnumCharacter character)
    {
        if (this.isInDebugMode) Debug.Log("Getting Dialouge for: " + character.ToString());

        DialogTree dialogTree = dialogTreesDict[character];

        if (this.isInDebugMode) Debug.Log("Found DialogTree: " + dialogTree.ToString());

        switch (character)
        {
            case EnumCharacter.LifeGauard:
                if (!GameManager.Instance.startedGameFinished && GameManager.Instance.startedGame)
                {
                    if (this.isInDebugMode) Debug.Log("Returning started game dialogue");
                    GameManager.Instance.startedGameFinished = true;
                    return dialogTree.storyDialouges[0];

                }
                else if (!GameManager.Instance.firstTreasureFinished && GameManager.Instance.firstTreasureCollected)
                {
                    if (this.isInDebugMode) Debug.Log("Returning first treasure dialogue");
                    GameManager.Instance.firstTreasureFinished = true;
                    return dialogTree.storyDialouges[1];

                }
                else if (!GameManager.Instance.firstStoryFinished && GameManager.Instance.firstStoryCollected)
                {
                    if (this.isInDebugMode) Debug.Log("Returning first story dialogue");
                    GameManager.Instance.firstStoryFinished = true;
                    return dialogTree.storyDialouges[2];

                }
                else if(!GameManager.Instance.firstSuitUpgradeFinished && GameManager.Instance.firstSuitUpgrade)
                {
                    if (this.isInDebugMode) Debug.Log("Returning first upgrade dialogue");
                    GameManager.Instance.firstSuitUpgradeFinished = true;
                    return dialogTree.storyDialouges[3];
                }
                else
                {
                    int randomIndex = Random.Range(0, dialogTree.genericDialouges.Count);
                    if (this.isInDebugMode) Debug.Log("Returning generic dialogue at index: " + randomIndex);
                    return dialogTree.genericDialouges[randomIndex];
                }

            default:
                Dialog deafualtString = null;
                List<string> list = new()
                {
                    "Missing Dialog"
                };
                deafualtString.dialogLines = list;

                return deafualtString;
        }
    }
}

[System.Serializable]
public struct DialogTree
{
    public EnumCharacter character;
    public List<Dialog> storyDialouges;
    public List<Dialog> genericDialouges;

    public DialogTree(List<Dialog> storyDialouges, List<Dialog> genericDialouges, EnumCharacter character)
    {
        this.storyDialouges = storyDialouges;
        this.genericDialouges = genericDialouges;
        this.character = character;
    }

    public override string ToString()
    {
        return $"Character: {character}, Story Dialogues: {storyDialouges.Count}, Generic Dialogues: {genericDialouges.Count}";
    }
}

[System.Serializable]
public class Dialog
{
    public List<string> dialogLines;

    public string[] ConvertToArray()
    {
        return dialogLines.ToArray();
    }
}

public enum EnumCharacter { LifeGauard }
