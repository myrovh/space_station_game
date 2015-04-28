﻿using UnityEngine;
using System.Collections.Generic;

public class ui : MonoBehaviour
{
    //reference to a test unit
    private List<GameObject> selectedUnitList = new List<GameObject>();
    public GameObject controlResource;
    private Vector3 startClick = -Vector3.one;
    public Texture2D selectionHighlight = null;
    public static Rect selection = new Rect(0, 0, 0, 0);

    //Test for input data (gives 2 orders to control unit)
    void Start()
    {
    }

    //Takes an order as a parameter and adds it to the units queue
    void addToQueue(Vector3 moveTo, data.unitAction actAt)
    {
        foreach (GameObject unit in selectedUnitList)
        {
            unit.GetComponent<unit>().queueOrder(moveTo, actAt);
        }
    }

    void selecionCheck()
    {
        GameObject[] allPlayerUnits;
        allPlayerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
        if (Input.GetButton("Select"))
        {
            foreach (GameObject unit in allPlayerUnits)
            {
                Vector3 unitPos = Camera.main.WorldToScreenPoint(unit.transform.position);
                unitPos.y = InvertMouseY(unitPos.y);

                if (selection.Contains(unitPos))
                {
                    selectedUnitList.Add(unit);
                    unit.GetComponent<unit>().selectionStatus(true);
                }
            }

        }

        if (Input.GetButtonUp("Select"))
        {
            RaycastHit hit;
            foreach (GameObject unit in selectedUnitList)
            {

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    if (hit.collider.tag == "PlayerUnit")
                    {
                        selectedUnitList.Add(unit);
                        unit.GetComponent<unit>().selectionStatus(true);
                    }
                }
                else
                {
                    unit.GetComponent<unit>().selectionStatus(false);
                    selectedUnitList.Clear();
                    startClick = -Vector3.one;
                }
            }
        }


    }

    void generateOrders()
    {
        foreach (GameObject unit in selectedUnitList)
        {
            if (Input.GetButton("Interact") && Input.GetButton("Queue") && unit.GetComponent<unit>().isSelected)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    addToQueue(hit.point, data.unitAction.STAND);
                }
            }
            else if (Input.GetButton("Interact") && unit.GetComponent<unit>().isSelected)
            {
                unit.GetComponent<unit>().clearQueue();
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    if (hit.collider.tag == "Resource")
                    {
                        unit.GetComponent<unit>().target = controlResource;
                        addToQueue(hit.point, data.unitAction.PICKUP);
                    }
                    else
                    {
                        addToQueue(hit.point, data.unitAction.STAND);
                    }
                }
                if (unit.GetComponent<unit>().isCarrying)
                {
                    addToQueue(hit.point, data.unitAction.DROP);
                }
            }
        }
    }

    void Update()
    {
        checkCamera();

        selecionCheck();

        generateOrders();
    }

    //Checking if the select button is being pressed and setting the values of the selection box
    //based on the start click and current mouse position
    void checkCamera()
    {
        if (Input.GetButtonDown("Select"))
        {
            startClick = Input.mousePosition;
        }

        if (Input.GetButton("Select"))
        {
            selection = new Rect(startClick.x, InvertMouseY(startClick.y), Input.mousePosition.x - startClick.x, InvertMouseY(Input.mousePosition.y) - InvertMouseY(startClick.y));

            if (selection.width < 0)
            {
                selection.x += selection.width;
                selection.width = -selection.width;
            }
            if (selection.height < 0)
            {
                selection.y += selection.height;
                selection.height = -selection.height;
            }
        }
    }

    //Drawing the selection box to the screen
    private void OnGUI()
    {
        if (startClick != -Vector3.one)
        {
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.DrawTexture(selection, selectionHighlight);
        }
    }

    public static float InvertMouseY(float y)
    {
        return Screen.height - y;
    }
}