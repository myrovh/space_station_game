using UnityEngine;
using System.Collections.Generic;

public class module : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public List<GameObject> ResourceSlots;
    public float SlotSize = 1.0f;
    public List<GameObject> unitsInModule;
    private Color startcolor;

    public bool noAir = false;
    private GameObject _onLights;
    private GameObject _offLights;
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

        if (_offLights.activeInHierarchy)
        {
            _offLights.transform.rotation = _offLights.transform.rotation*Quaternion.Euler(0, 50*Time.deltaTime, 0);
        }


        if(Input.GetKey("space"))
        {
            eventManager.depressurizeModule();
        }

        //Debug check to test module light switching
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetPowerState(true);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            SetPowerState(false);
        }
    }

    void OnDisable()
    {
        eventManager.depressurize -= this.depressurizeModule;
    }
    #endregion

    public void Start()
    {
        _onLights = transform.Find("on").gameObject;
        _offLights = transform.Find("off").gameObject;

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

    public void SetPowerState(bool isPower)
    {
        _onLights.SetActive(isPower);
        _offLights.SetActive(!isPower);
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