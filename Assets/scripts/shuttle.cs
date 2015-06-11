using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class shuttle : module
{
    private Vector3 currentDestination = -Vector3.one;
    private Vector3 currentPosition;
    private float journeyLength = -1;
    private float speed = 1.0F;
    private float startTime;
    public GameObject shuttleDoor;
    private bool shuttleMove = true;
    private bool shuttleDocked = false;
    private bool levelExit = false;

    void Start()
    {
        base.Start();
    }

    public List<GameObject> GetFreeSlots()
    {
        List<GameObject> freeSlots = new List<GameObject>();
        base.GetFreeSlots();
        return freeSlots;
    }
    void Update()
    {
        if (currentDestination != -Vector3.one)
        {
            currentPosition = transform.position;
            if (journeyLength == -1)
            {
                journeyLength = Vector3.Distance(currentPosition, currentDestination);
            }

            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;

            if (!levelExit)
            {
                if (shuttleMove)
                {
                    if (Vector3.Distance(currentPosition, currentDestination) > .1f)
                    {
                        transform.position = Vector3.Lerp(currentPosition, currentDestination, fracJourney);
                    }
                    else
                    {
                        if (!shuttleDoor.GetComponent<Door>().IsOpen && shuttleDocked)
                        {
                            shuttleDoor.GetComponent<Door>().StartDoorOpen();
                            shuttleMove = false;
                            resetLengthAndTime();
                        }
                        if (!shuttleDocked)
                        {
                            dockShuttle();
                            shuttleDocked = true;
                        }
                    }
                }
            }
            else
            {
                if (shuttleDocked)
                {
                    resetLengthAndTime();
                    StartCoroutine(unDockShuttle());
                }
                else
                {
                    if (Vector3.Distance(currentPosition, currentDestination) > .1f)
                    {
                        transform.position = Vector3.Lerp(currentPosition, currentDestination, fracJourney);
                    }
                    else
                    {
                        levelEnd();
                    }
                }
            }
        }
    }

    public void enterLevel()
    {
        startTime = Time.time;
        currentDestination = GameObject.FindGameObjectWithTag("dock").transform.position;
        currentDestination.z -= 2.0f;
    }

    public void exitLevel()
    {
        levelExit = true;
    }
    void levelEnd()
    {
        resetLengthAndTime();
        currentDestination = new Vector3(-30.0f, -2.76f, -41.73f);
        shuttleMove = true;
        shuttleDocked = false;
        StartCoroutine(levelTransition());
        //Application.LoadLevel("start_menu");
    }

    void resetLengthAndTime()
    {
        journeyLength = -1;
        startTime = Time.time;
    }

    void dockShuttle()
    {
        currentDestination.z = currentDestination.z + 2;
    }

    IEnumerator unDockShuttle()
    {
        shuttleDoor.GetComponent<Door>().StartDoorClose();
        currentDestination.z = currentPosition.z - 2.0f;
        yield return new WaitForSeconds(2);
        shuttleDocked = false;
    }


    IEnumerator levelTransition()
    {
        yield return new WaitForSeconds(3);
        Application.LoadLevel("start_menu");
    }
}
