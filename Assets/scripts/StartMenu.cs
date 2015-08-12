using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class StartMenu : MonoBehaviour {

    public void TransitionLevel()
    {
        Application.LoadLevel("control_menu");
    }

	public void TransitionOptions()
	{
		Application.LoadLevel("options_menu");

	}
}