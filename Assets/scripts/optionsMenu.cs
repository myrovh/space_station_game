using UnityEngine;
using System.Collections;

public class optionsMenu : MonoBehaviour 
{
	public void TransitionMainMenu ()
	{
		Application.LoadLevel ("start_menu");
	}

	public void TransitionControlsMenu ()
	{
		Application.LoadLevel ("control_menu");
	}

}
