using UnityEngine;
using System.Collections;

public class level1Script : MonoBehaviour {

    public GameObject shuttle;
    public GameObject unit1;
    public GameObject unit2;

	void Start () 
    {
        shuttle.GetComponent<shuttle>().enterLevel();

        unit1.GetComponent<unit>().SpeakDialogue(Dialogue.GetRandomDialogueType(Dialogue.DialogueType.MOVEORDER));
	}
	
	void Update () 
    {
	
	}
}
