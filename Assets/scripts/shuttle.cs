using UnityEngine;
using System.Collections;

public class shuttle : MonoBehaviour {

    Vector3 currentDestination = Vector3.zero;
    Vector3 currentPosition;
    public GameObject shuttleDoor;
    bool shuttleMove = true;
    float speed = 7;
	
	void Update () 
    {
        currentPosition = transform.position;
        if (shuttleMove)
        {
            if (Vector3.Distance(currentPosition, currentDestination) > .1f)
            {
                Vector3 directionOfTravel = currentDestination - currentPosition;
                directionOfTravel.Normalize();

                this.transform.Translate(
                (directionOfTravel.x * speed * Time.deltaTime),
                (directionOfTravel.y * speed * Time.deltaTime),
                (directionOfTravel.z * speed * Time.deltaTime),
                Space.World);
            }
            else
            {
                if (!shuttleDoor.GetComponent<Door>().IsOpen)
                {
                    shuttleDoor.GetComponent<Door>().StartDoorOpen();
                }
                shuttleMove = false;
            }
        }
        StartCoroutine(levelEnd());
	}

    IEnumerator levelEnd()
    {
        if (Input.GetKey("m"))
        {
            shuttleDoor.GetComponent<Door>().StartDoorClose();
            yield return new WaitForSeconds(2);
            currentDestination = new Vector3(-17.7f, 0.0f, -16.3f);
            shuttleMove = true;
        }
    }
}
