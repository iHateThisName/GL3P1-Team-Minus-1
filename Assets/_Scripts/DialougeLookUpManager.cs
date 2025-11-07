using UnityEngine;
using System.Collections.Generic;

public class DialougeLookUpManager : MonoBehaviour
{
    [SerializeField] private List<DialogTree> dialogTrees;

    private Dictionary<EnumCharacter, DialogTree> dialogTreesDict;

    [SerializeField]
    private bool startedGame = true;
    [SerializeField]
    private bool startedGameFinished;
    [SerializeField]
    private bool firstTreasureCollected;
    [SerializeField]
    private bool firstTreasureFinished;


    private void Start()
    {
        foreach (var Tree in dialogTrees)
        {
            dialogTreesDict.Add(Tree.character, Tree);
        }
    }

    public string[] GetDialouge(EnumCharacter character)
    {



        //DialogTree dialogTree = GetCharactersDialogTree(character);

        DialogTree dialogTree = dialogTreesDict[character];

        switch (character)
        {
            case EnumCharacter.LifeGauard:


                //if (!startedGameFinished && startedGame)
                //{
                //    return storyDialougeList[0];
                //}
                //else if (!firstTreasureFinished && firstTreasureCollected)
                //{
                //    return storyDialougeList[1];
                //}
                //else
                //{
                //    return genericDialougeScript[0];
                //}

                return dialogTree.storyDialouges[0].ToArray();

            default:
                string[] deafualtString = null;
                deafualtString[0] = "Missing Dialog Text";

                return deafualtString;
        }
    }

    private DialogTree GetCharactersDialogTree(EnumCharacter lookUpCharacter)
    {

        foreach (var dialogTree in dialogTrees)
        {
            if (dialogTree.character == lookUpCharacter)
            {
                return dialogTree;
            }
        }

        return new DialogTree();
    }
}

[System.Serializable]
public struct DialogTree
{
    public List<List<string>> storyDialouges;
    public List<List<string>> genericDialouges;
    public EnumCharacter character;

    public DialogTree(List<List<string>> storyDialouges, List<List<string>> genericDialouges, EnumCharacter character)
    {
        this.storyDialouges = storyDialouges;
        this.genericDialouges = genericDialouges;
        this.character = character;
    }
}

public enum EnumCharacter { LifeGauard}
