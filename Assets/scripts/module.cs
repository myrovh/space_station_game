using UnityEngine;
using System.Collections.Generic;

public class Module : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public List<GameObject> ResourceSlots;
    public float SlotSize = 1.0f;
    public List<GameObject> unitsInModule;

    public bool noAir = false;
    #endregion

    #region Event System Functions
    void OnEnable()
    {
        eventManager.depressurize += this.depressurizeModule;
    }

    void depressurizeModule()
    {
            noAir = true;
    }

    void Update()
    {
        if (noAir)
        {
            foreach (GameObject currentUnit in unitsInModule)
            {
                currentUnit.GetComponent<unit>().takeDamage();
            }
        }


        if(Input.GetKey("space"))
        {
            eventManager.depressurizeModule();
        }
    }

    void OnDisable()
    {
        eventManager.depressurize -= this.depressurizeModule;
    }
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

    #region Unit List Manipulation
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerUnit")
            unitsInModule.Add(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        unitsInModule.Remove(other.gameObject);
    }
    #endregion
}