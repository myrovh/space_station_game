using UnityEngine;
using System.Collections.Generic;

public class ui : MonoBehaviour
{
    List<GameObject> allPlayerUnits = new List<GameObject>();
    private GameObject targetResource;
    public GameObject popup;
    private GameObject currentUnit;
    private Vector3 orderPos;
    private Vector3 startClick = -Vector3.one;
    public Texture2D selectionHighlight = null;
    public static Rect selection = new Rect(0, 0, 0, 0);
    private RaycastHit hit;

    [SerializeField]
    public UnityEngine.UI.Button moveButton = null;
    [SerializeField]
    public UnityEngine.UI.Button pickUpButton = null;
    [SerializeField]
    public UnityEngine.UI.Button dropButton = null;

    void Start()
    {
        //Adding listeners for each of the buttons

        moveButton.onClick.AddListener(() => { moveOrder(); });
        pickUpButton.onClick.AddListener(() => { pickUpOrder(); });
        dropButton.onClick.AddListener(() => { dropOrder(); });

        allPlayerUnits.AddRange(GameObject.FindGameObjectsWithTag("PlayerUnit"));
    }

    void moveOrder()
    {
        addToQueue(orderPos, data.unitAction.STAND, null, currentUnit);
        showOrders(false);
    }

    void pickUpOrder()
    {
        currentUnit.GetComponent<unit>().target = targetResource;
        addToQueue(Vector3.zero, data.unitAction.PICKUP, targetResource, currentUnit);
        showOrders(false);
    }

    void dropOrder()
    {
        addToQueue(orderPos, data.unitAction.DROP, null, currentUnit);
        showOrders(false);
    }

    //Enables or disables unit order popup based on parameter given
    void showOrders(bool isVisible)
    {
        if (isVisible)
        {
            popup.GetComponent<CanvasGroup>().alpha = 1;
            popup.GetComponent<RectTransform>().position = Input.mousePosition;
        }
        else
        {
            popup.GetComponent<CanvasGroup>().alpha = 0;
        }

        popup.GetComponent<CanvasGroup>().interactable = isVisible;
    }

    //Takes an order as a parameter and adds it to the units queue
    void addToQueue(Vector3 moveTo, data.unitAction actAt, GameObject actAtObject, GameObject unit)
    {
        if (moveTo != Vector3.zero)
        {
            unit.GetComponent<unit>().queueOrder(moveTo, actAt);
        }
        else
        {
            unit.GetComponent<unit>().queueOrder(actAtObject, actAt);
        }
        
    }

    void selecionCheck()
    {
        foreach (GameObject unit in allPlayerUnits)
        {
            if (Input.GetButton("Select"))
            {
                Vector3 unitPos = Camera.main.WorldToScreenPoint(unit.transform.position);
                unitPos.y = InvertMouseY(unitPos.y);

                //Select unit if unit is inside selection box
                if (selection.Contains(unitPos))
                {
                    unit.GetComponent<unit>().selectionStatus(true);
                }
            }

            if (Input.GetButtonDown("Select"))
            {
                //Selects unit if is clicked while underneath mouse cursor
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) && hit.collider.tag == "PlayerUnit")
                {
                    unit.GetComponent<unit>().selectionStatus(false);
                    hit.rigidbody.GetComponent<unit>().selectionStatus(true);
                }
                //Deselects all units that are not hit by raycast
                else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) && hit.collider.tag != "PopupMenu")
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
            if (Input.GetButtonDown("Interact") && Input.GetButton("Queue") && unit.GetComponent<unit>().isSelected)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    if (hit.collider.tag == "Resource")
                    {
                        showOrders(true);
                        orderPos = hit.point;
                        currentUnit = unit;
                        targetResource = hit.collider.gameObject;
                    }
                    else
                    {
                        if (unit.GetComponent<unit>().isCarrying)
                        {
                            showOrders(true);
                            orderPos = hit.point;
                            currentUnit = unit;
                        }
                        else
                        {
                            addToQueue(hit.point, data.unitAction.STAND, null, unit);
                        }
                    }
                }
            }
            else if (Input.GetButtonDown("Interact") && unit.GetComponent<unit>().isSelected)
            {
               unit.GetComponent<unit>().clearQueue();
               if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    if (hit.collider.tag == "Resource")
                    {
                            showOrders(true);
                            orderPos = hit.point;
                            currentUnit = unit;
                            targetResource = hit.collider.gameObject;
                    }
                    else
                    {
                        if (unit.GetComponent<unit>().isCarrying)
                        {
                            showOrders(true);
                            orderPos = hit.point;
                            currentUnit = unit;
                        }
                        else
                        {
                            addToQueue(hit.point, data.unitAction.STAND, null, unit);
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        checkSelectionBox();

        selecionCheck();

        generateOrders();

    }

    //Checking if the select button is being pressed and setting the values of the selection box
    //based on the start click and current mouse position
    void checkSelectionBox()
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