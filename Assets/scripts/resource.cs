using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class resource : MonoBehaviour
{
    public bool isCarried = false;
    GameObject owner;
    public data.resourceType resourceType;
    public List<data.interactions> interactions;
    public Vector3 dropPosition;

    void Start()
    {
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        interactions.Add(data.interactions.HAUL);
    }

    void Update()
    {
        if (isCarried)
        {
            transform.position = (owner.transform.position + owner.transform.up);
        }
    }

    public void PickedUp(GameObject newOwner)
    {
        owner = newOwner;
        isCarried = true;
    }

    public void Dropped()
    {
        transform.position = dropPosition;
        owner = null;
        isCarried = false;
    }

    public void DroppedInFront()
    {
        transform.position = owner.transform.position + Vector3.forward;
        owner = null;
        isCarried = false;
    }

    public List<data.interactions> getInteractions()
    {
        return interactions;
    }
}