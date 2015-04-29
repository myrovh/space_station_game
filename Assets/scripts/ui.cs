using UnityEngine;
using System.Collections.Generic;

public class ui : MonoBehaviour
{
    //reference to a test unit
    //private List<GameObject> selectedUnitList = new List<GameObject>();
    private GameObject[] allPlayerUnits;
    public GameObject controlResource;
    private Vector3 startClick = -Vector3.one;
    public Texture2D selectionHighlight = null;
    public static Rect selection = new Rect(0, 0, 0, 0);

    //Test for input data (gives 2 orders to control unit)
    void Start()
    {
        allPlayerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
    }

    //Takes an order as a parameter and adds it to the units queue
    void addToQueue(Vector3 moveTo, data.unitAction actAt, GameObject unit)
    {
        unit.GetComponent<unit>().queueOrder(moveTo, actAt);
    }

    void selecionCheck()
    {
        foreach (GameObject unit in allPlayerUnits)
        {
            if (Input.GetButton("Select"))
            {

                Vector3 unitPos = Camera.main.WorldToScreenPoint(unit.transform.position);
                unitPos.y = InvertMouseY(unitPos.y);

                if (selection.Contains(unitPos))
                {
                    unit.GetComponent<unit>().selectionStatus(true);
                }
            }
            if (Input.GetButtonDown("Select"))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    if (hit.collider.tag == "PlayerUnit")
                    {
                        unit.GetComponent<unit>().selectionStatus(false);
                        hit.rigidbody.GetComponent<unit>().selectionStatus(true);
                    }
                    else
                    {
                        unit.GetComponent<unit>().selectionStatus(false);
                    }
                }
                else
                {
                    unit.GetComponent<unit>().selectionStatus(false);
                }
            }
        }


    }

    void generateOrders()
    {
        foreach (GameObject unit in allPlayerUnits)
        {
            if (Input.GetButton("Interact") && Input.GetButton("Queue") && unit.GetComponent<unit>().isSelected)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    addToQueue(hit.point, data.unitAction.STAND, unit);
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
                        addToQueue(hit.point, data.unitAction.PICKUP, unit);
                    }
                    else
                    {
                        addToQueue(hit.point, data.unitAction.STAND, unit);
                    }
                }
                if (unit.GetComponent<unit>().isCarrying)
                {
                    addToQueue(hit.point, data.unitAction.DROP, unit);
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
        else if (Input.GetButtonUp("Select"))
        {
            startClick = -Vector3.one;
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