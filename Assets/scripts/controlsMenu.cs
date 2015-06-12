using UnityEngine;
using System.Collections;

public class controlsMenu : MonoBehaviour 
{

	void Update () 
    {
	    if (Input.anyKey)
        {
            Application.LoadLevel("level_1_hardpoints");
        }
	}
}
