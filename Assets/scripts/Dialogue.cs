using UnityEngine;
using System.Collections.Generic;

public static class Dialogue
{
    //Dialogue Type
    public enum DialogueType
    {
        MANUAL
    };

    public static List<DialogueText> List = new List<DialogueText>();

    public static void BuildDialogue()
    {
        List.Add(new DialogueText(0, DialogueType.MANUAL, "Test String"));
        List.Add(new DialogueText(1, DialogueType.MANUAL, "Second Test String"));
    }

    public static void ReportDialogue()
    {
        foreach (DialogueText a in List)
        {
            Debug.Log(a.DialogueId + " " + a.DialogueContents);
        }
    }

    /*
    public static AudioSource GetDialogueAudio(DialogueText text)
    {
        
    }
    */
}

public class DialogueText
{
    public int DialogueId;
    public Dialogue.DialogueType DialogueType;
    public string DialogueContents;

    public DialogueText(int id, Dialogue.DialogueType type, string contents)
    {
        DialogueId = id;
        DialogueType = type;
        DialogueContents = contents;
    }
}