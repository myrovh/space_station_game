using UnityEngine;
using System.Collections;

public class ui : MonoBehaviour
{
    //reference to a test unit
    public GameObject controlUnit;
    private Vector3 startClick = -Vector3.one;
    public Texture2D selectionHighlight = null;
    public static Rect selection = new Rect(0, 0, 0, 0);

    //Test for input data (gives 2 orders to control unit)
    void Start()
    {
    }

    //Takes an order as a parameter and adds it to the units queue
    void addToQueue(data.unitOrder tempOrder)
    {
        controlUnit.GetComponent<unit>().queueOrder(tempOrder);
    }

    void Update()
    {
        CheckCamera();

        if (Input.GetButton("Select"))
        {
            if (controlUnit.GetComponent<unit>().selectedByClick != true)
            {
                Vector3 camPos = Camera.main.WorldToScreenPoint(controlUnit.transform.position);
                camPos.y = InvertMouseY(camPos.y);
                controlUnit.GetComponent<unit>().SelectionStatus(selection.Contains(camPos));
            }
        }

        if (Input.GetButton("Interact") && Input.GetButton("Queue") && controlUnit.GetComponent<unit>().isSelected)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                data.unitOrder tempOrder = new data.unitOrder(hit.point, data.unitAction.STAND);
                addToQueue(tempOrder);
            }
        }
        else if (Input.GetButton("Interact") && controlUnit.GetComponent<unit>().isSelected)
        {
            controlUnit.GetComponent<unit>().clearQueue();
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                data.unitOrder tempOrder = new data.unitOrder(hit.point, data.unitAction.STAND);
                addToQueue(tempOrder);
            }
        }
    }

    //Checking if the select button is being pressed and setting the values of the selection box
    //based on the start click and current mouse position
    void CheckCamera()
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