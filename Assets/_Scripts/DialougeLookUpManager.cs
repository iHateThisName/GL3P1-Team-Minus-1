using Assets.Scripts.Singleton;
using System.Collections.Generic;
using UnityEngine;

public class DialougeLookUpManager : Singleton<DialougeLookUpManager>
{
    [SerializeField] private List<DialogTree> dialogTrees;

    private Dictionary<EnumCharacter, DialogTree> dialogTreesDict = new Dictionary<EnumCharacter, DialogTree>();

    private void Awake()
    {
        foreach (DialogTree dialogTree in dialogTrees)
        {
            dialogTreesDict.Add(dialogTree.character, dialogTree);
        }
    }

    public string[] GetDialouge(EnumCharacter character)
    {
        //DialogTree dialogTree = GetCharactersDialogTree(character);

        DialogTree dialogTree = dialogTreesDict[character];

        switch (character)
        {
            case EnumCharacter.LifeGauard:
                if(!GameManager.Instance.startedGameFinished && GameManager.Instance.startedGame)
                {
                    GameManager.Instance.startedGameFinished = true;
                    return dialogTree.storyDialouges[0].ConvertToArray();
                }
                else if(!GameManager.Instance.firstTreasureFinished && GameManager.Instance.firstTreasureCollected)
                {
                    return dialogTree.storyDialouges[1].ConvertToArray();
                }
                else
                {
                    return dialogTree.genericDialouges[Random.Range(0, dialogTree.genericDialouges.Count)].ConvertToArray();
                }

            default:
                string[] deafualtString = null;
                deafualtString[0] = "Missing Dialog Text";

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

public enum EnumCharacter { LifeGauard}
