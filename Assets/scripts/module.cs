using UnityEngine;
using System.Collections.Generic;

public class module : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public List<GameObject> resourceSlots;
    public float slotSize = 2.0f;
    #endregion

    void Start () {
        // Add slot_# empties to the resourceSlots list
        Transform hardpointList = transform.Find("hardpoints");
        foreach (Transform child in hardpointList)
        {
            resourceSlots.Add(child.gameObject);
        }
	}

    // Returns list containing gameobjects of any slots that don't have a resource too close
    List<GameObject> getFreeSlots()
    {
        List<GameObject> freeSlots = null;
        GameObject[] allResources = GameObject.FindGameObjectsWithTag("Resource");

        foreach (GameObject slot in resourceSlots) {
            bool isFree = true;
            for (int i = 0; i < allResources.Length; i++)
            {
                if ((slot.transform.position - allResources[i].transform.position).magnitude < slotSize)
                {
                    isFree = false;
                }
            }

            if (isFree)
            {
                freeSlots.Add(slot);
            }
        }

        return freeSlots;
    }
}