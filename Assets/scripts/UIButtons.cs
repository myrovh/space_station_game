using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class UIButtons : MonoBehaviour {

    public void TransitionLevelSelect()
    {
        Application.LoadLevel("level_select_menu");
    }

	public void TransitionOptions()
	{
		Application.LoadLevel("options_menu");

	}

	public void TransitionPlayLevel()
	{
		Application.LoadLevel ("Level_1_hardpoints");
	}
	
	public void TransitionMainMenu()
	{
		Application.LoadLevel ("start_menu");
	}

	public void TransitionControlsMenu ()
	{
		Application.LoadLevel ("control_menu");
	}
	
	public void TransitionPreviewLevel ()
	{
		Application.LoadLevel ("level_preview_menu");
	}
}