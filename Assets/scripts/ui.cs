using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class ui : MonoBehaviour
{
    #region Variables

    //Unit Selection Vairables
    public Texture2D selectionHighlight = null;
    public static Rect selection = new Rect(0, 0, 0, 0);
    public bool buttonClick = false;
    private List<GameObject> allPlayerUnits = new List<GameObject>();
    private RaycastHit hit;
    private Vector3 startClick = -Vector3.one;

    //Haul Order Variables
    private GameObject targetResource;
    private bool haulOrder = false;
    private List<GameObject> freeSlots;

    //Popup Variables
    [SerializeField]
    public UnityEngine.UI.Button button1 = null;
    [SerializeField]
    public UnityEngine.UI.Button button2 = null;
    [SerializeField]
    public UnityEngine.UI.Button button3 = null;
    public GameObject popup;
    private GameObject currentUnit;

    #endregion

    #region Monobehaviour Functions
    void Start()
    {
        //Adding listeners for each of the buttons

        //button1.onClick.AddListener(() => { button1Action(); });
        //button2.onClick.AddListener(() => { button2Action(); });
        //button3.onClick.AddListener(() => { button3Action(); });

        allPlayerUnits.AddRange(GameObject.FindGameObjectsWithTag("PlayerUnit"));
    }
    void Update()
    {
        checkSelectionBox();

        selecionCheck();

        generateOrders();
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
    #endregion

    #region Generate Order Functions
    //Generates orders for currently selected units
    void generateOrders()
    {
        if (!haulOrder)
        {
            foreach (GameObject unit in allPlayerUnits)
            {
                if (Input.GetButtonDown("Interact") && Input.GetButton("Queue") && unit.GetComponent<unit>().isSelected)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        if (hit.collider.tag == "Resource")
                        {
                            currentUnit = unit;
                            targetResource = hit.collider.gameObject;
                            haulOrder = true;
                        }
                        else
                        {
                            addToQueue(hit.point, data.unitAction.STAND, null, unit);
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
                            currentUnit = unit;
                            targetResource = hit.collider.gameObject;
                            if (targetResource.GetComponent<resource>().interactions.Count <= 1)
                            {
                                haulOrder = true;
                            }
                        }
                        else if (hit.collider.tag == "door")
                        {
                            currentUnit = unit;
                            currentDoor = hit.transform.parent.gameObject;
                            currentUnit.GetComponent<unit>().door = currentDoor;
                            if (!currentDoor.GetComponent<Door>().isOpen)
                            {
                                progressBar.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
                                addToQueue(Vector3.zero, data.unitAction.OPENDOOR, currentDoor, currentUnit);
                            }
                            else
                            {
                                progressBar.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
                                addToQueue(Vector3.zero, data.unitAction.CLOSEDOOR, currentDoor, currentUnit);
                            }
                        }
                        else
                        {
                            addToQueue(hit.point, data.unitAction.STAND, null, unit);
                        }
                    }
                }
            }
        }
        else
        {

            if (Input.GetButtonDown("Interact") && Input.GetButton("Queue"))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    freeSlots = hit.transform.root.GetComponent<module>().getFreeSlots();
                    targetResource.GetComponent<resource>().dropPosition = freeSlots[0].transform.position;
                    addToQueue(Vector3.zero, data.unitAction.PICKUP, targetResource, currentUnit);
                    addToQueue(hit.point, data.unitAction.DROP, null, currentUnit);
                    haulOrder = false;
                }
            }
            else if (Input.GetButtonDown("Interact"))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    freeSlots = hit.transform.root.GetComponent<module>().getFreeSlots();
                    targetResource.GetComponent<resource>().dropPosition = freeSlots[0].transform.position;
                    addToQueue(Vector3.zero, data.unitAction.PICKUP, targetResource, currentUnit);
                    addToQueue(hit.point, data.unitAction.DROP, null, currentUnit);
                    haulOrder = false;
                }
            }
        }
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
            currentUnit.GetComponent<unit>().target = targetResource;
            unit.GetComponent<unit>().queueOrder(actAtObject, actAt);
        }

    }
    #endregion

    #region Unit Selection Functions
    //Function to check if units are being selected
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
        }

        if (Input.GetButtonDown("Select") && !buttonClick)
        {
            buttonClick = true;
            //Selects unit if is clicked while underneath mouse cursor
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) && hit.collider.tag == "PlayerUnit")
            {
                foreach (GameObject unit in allPlayerUnits)
                {
                    unit.GetComponent<unit>().selectionStatus(false);
                }
                hit.rigidbody.GetComponent<unit>().selectionStatus(true);
                showOrders(false);
            }
            //Deselects all units that are not hit by raycast
            else
            {
                //Check to see if the mouse pointer is over a ui object
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    foreach (GameObject unit in allPlayerUnits)
                    {
                        unit.GetComponent<unit>().selectionStatus(false);
                        showOrders(false);
                    }
                }
            }
        }

        if (Input.GetButtonUp("Select"))
        {
            buttonClick = false;
            haulOrder = false;
        }
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
    //Inverts the y value of the mouse position
    public static float InvertMouseY(float y)
    {
        return Screen.height - y;
    }
    #endregion

    #region Context Popup Functions
    void button1Action()
    {

    }

    void button2Action()
    {

    }

    void button3Action()
    {

    }

    //Enables or disables unit order popup based on parameter given
    void showOrders(bool isVisible)
    {
        if (isVisible)
        {
            popup.GetComponent<CanvasGroup>().alpha = 1;
            popup.GetComponent<CanvasGroup>().blocksRaycasts = true;
            popup.GetComponent<RectTransform>().position = Input.mousePosition;
        }
        else
        {
            popup.GetComponent<CanvasGroup>().blocksRaycasts = false;
            popup.GetComponent<CanvasGroup>().alpha = 0;
        }

        popup.GetComponent<CanvasGroup>().interactable = isVisible;
    }
    #endregion
}