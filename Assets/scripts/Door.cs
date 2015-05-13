using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
    Animator doorAnimation;
    int openHash = Animator.StringToHash("opening");
    int closeHash = Animator.StringToHash("closing");
    public bool isOpen;
    public int doorOpenTime;

	// Use this for initialization
	void Start () {
        doorAnimation = GetComponent<Animator>();
        isOpen = false;
	}

    public void startDoorOpen()
    {
        doorAnimation.SetTrigger(openHash);
        isOpen = true;
    }

    public void startDoorClose()
    {
        doorAnimation.SetTrigger(closeHash);
        isOpen = false;
    }

    public bool isDoorOpen()
    {
        return isOpen;
    }
}
