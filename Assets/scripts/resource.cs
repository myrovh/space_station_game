using UnityEngine;
using System.Collections;

public class resource : MonoBehaviour
{
    public bool isCarried = false;
    GameObject owner;

    void Start()
    {
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void PickedUp(GameObject newOwner)
    {
        owner = newOwner;
        isCarried = true;
        gameObject.SetActive(false);
    }

    public void Dropped()
    {
        transform.position = (owner.transform.position + owner.transform.forward);
        owner = null;
        isCarried = false;
        gameObject.SetActive(true);
    }
}