using UnityEngine;
using System.Collections;

public class controlsMenu : MonoBehaviour 
{

	
	public void TransitionPlayLevel()
	{
		Application.LoadLevel ("Level_1_hardpoints");
	}

	public void TransitionMainMenu()
	{
		Application.LoadLevel ("start_menu");
	}
}
