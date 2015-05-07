using UnityEngine;
using System.Collections.Generic;

public class module : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public List<GameObject> resourceSlots;
    #endregion

    void Start () {
        // Add slot_# empties to the resourceSlots list
        Transform hardpointList = transform.Find("hardpoints");
        foreach (Transform child in hardpointList)
        {
            resourceSlots.Add(child.gameObject);
        }
	}
}