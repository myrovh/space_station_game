using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
    Animator doorAnimation;
    int openHash = Animator.StringToHash("opening");
    int closeHash = Animator.StringToHash("closing");
    public bool isOpen;
    public int doorOpenTime;

    float doorTimer = 5.0f;
    public bool unitUsingDoor = false;

	// Use this for initialization
	void Start () {
        doorAnimation = GetComponent<Animator>();
        isOpen = false;
	}

    void Update()
    {
        if (unitUsingDoor)
        {
            doorTimer -= Time.deltaTime;
            if (doorTimer <= 0 && !isOpen)
            {
                startDoorOpen();
                doorTimer = 5.0f;
                unitUsingDoor = false;
            }
            else if(doorTimer <= 0 && isOpen)
            {
                startDoorClose();
                doorTimer = 5.0f;
                unitUsingDoor = false;
            }
        }
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
