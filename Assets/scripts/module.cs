using UnityEngine;
using System.Collections.Generic;

public class Module : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public List<GameObject> ResourceSlots;
    public float SlotSize = 2.0f;
    #endregion

    void Start()
    {
        // Add slot_# empties to the resourceSlots list
        Transform hardpointList = transform.Find("hardpoints");
        foreach (Transform child in hardpointList)
        {
            ResourceSlots.Add(child.gameObject);
        }
    }

    // Returns list containing gameobjects of any slots that don't have a resource too close
    public List<GameObject> GetFreeSlots()
    {
        List<GameObject> freeSlots = new List<GameObject>();
        GameObject[] allResources = GameObject.FindGameObjectsWithTag("Resource");

        foreach (GameObject slot in ResourceSlots)
        {
            bool isFree = true;
            for (int i = 0; i < allResources.Length; i++)
            {
                float magnitudeCheck = (slot.transform.position - allResources[i].transform.position).magnitude;
                if (magnitudeCheck < SlotSize) 
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