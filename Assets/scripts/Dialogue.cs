using UnityEngine;
using System.Collections.Generic;

public static class Dialogue
{
    //Dialogue Type
    public enum DialogueType
    {
        MANUAL,
        MOVEORDER
    };

    public static List<DialogueText> DialogueList = new List<DialogueText>();
    public const string audioPath = "sounds";
    public static List<AudioClip> AudioList = new List<AudioClip>();

    public static void BuildDialogue()
    {
        DialogueList.Add(new DialogueText(0, DialogueType.MANUAL, "Test String"));
        DialogueList.Add(new DialogueText(1, DialogueType.MANUAL, "Second Test String"));
        DialogueList.Add(new DialogueText(2, DialogueType.MOVEORDER, "I'm on my way"));
        DialogueList.Add(new DialogueText(3, DialogueType.MOVEORDER, "Moving"));
        DialogueList.Add(new DialogueText(4, DialogueType.MOVEORDER, "Okay"));

        foreach (AudioClip a in Resources.LoadAll(audioPath, typeof(AudioClip)))
        {
            AudioList.Add(a);
        }
    }

    public static void ReportDialogue()
    {
        foreach (DialogueText a in DialogueList)
        {
            Debug.Log(a.DialogueId + " " + a.DialogueContents);
        }
    }

    public static DialogueText GetDialogueFromId(int id)
    {
        if (DialogueList[id] != null)
        {
            return DialogueList[id];
        }

        return null;
    }

    public static DialogueText GetRandomDialogueType(DialogueType category)
    {
        List<DialogueText> subList = new List<DialogueText>();
        foreach (DialogueText t in DialogueList)
        {
            if (t.DialogueCategory == category)
            {
                subList.Add(t);
            }
        }

        if (subList.Count > 0)
        {
            return subList[Random.Range(0, subList.Count)];
        }

        return null;
    }

    public static AudioClip GetDialogueAudio(DialogueText text)
    {
        foreach (AudioClip a in AudioList)
        {
            if (a.name == text.DialogueId.ToString())
            {
                return a;
            }
        }
        return null;
    }
}

public class DialogueText
{
    public int DialogueId;
    public Dialogue.DialogueType DialogueCategory;
    public string DialogueContents;

    public DialogueText(int id, Dialogue.DialogueType type, string contents)
    {
        DialogueId = id;
        DialogueCategory = type;
        DialogueContents = contents;
    }
}