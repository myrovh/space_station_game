using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class StartMenu : MonoBehaviour {

    public void TransitionLevel()
    {
        Application.LoadLevel("Level_1");
    }
}