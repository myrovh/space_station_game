using UnityEngine;
using System.Collections;

public class unitCarry : orderComponent 
{
    public bool isCarrying = false;
    private GameObject inventory;

    public override string getDescription()
    {
        return "carry";
    }

    //Function to make unit pick up the object passed as a parameter
    public void pickUp(GameObject newObject)
    {
        inventory = newObject;
        inventory.GetComponent<resource>().PickedUp(this.gameObject);
        isCarrying = true;
    }

    //Function to make the unit drop the resource object they are carrying
    //If the unit has not reached its destination i.e: the path is blocked
    //it will drop the resource object in front of it
    public void drop()
    {
        if (inventory != null)
        {
                inventory.GetComponent<resource>().Dropped();
                inventory = null;
                isCarrying = false;
        }
    }
}
