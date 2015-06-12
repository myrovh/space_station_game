using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class ui : MonoBehaviour
{
    #region Variables

    //Unit Selection Variables
    public Texture2D SelectionHighlight = null;
    private static Rect selection = new Rect(0, 0, 0, 0);
    private bool buttonClick = false;
    private List<GameObject> allPlayerUnits = new List<GameObject>();
    private RaycastHit hit;
    private Vector3 startClick = -Vector3.one;

    //Haul Order Variables
    private GameObject targetResource;
    private bool haulOrder = false;
    private List<GameObject> freeSlots;
    private int interactablesMask = (1 << 8);
    private Vector3 currentDestination;
    public GameObject moduleSelectionPrefab = null;
    private GameObject glowClone = null;
    private GameObject thisModule = null;

    //Popup Variables
    [SerializeField]
    public UnityEngine.UI.Button button1 = null;
    [SerializeField]
    public UnityEngine.UI.Button button2 = null;
    [SerializeField]
    public UnityEngine.UI.Button button3 = null;
    public GameObject popup;
    private GameObject currentUnit;
    private bool menuOpen = false;
    private int action1 = 0;
    private int action2 = 0;

    //Camera Variables
    public GameObject LevelCamera;
    private LevelCamera _cameraScript;
    private bool _cameraInit;

    // Door Variables
    private GameObject currentDoor;

    //Dialogue Variables
    public GameObject dialoguePrefab;

    //Textures
    public Texture2D cursorTexture;
    #endregion

    #region Monobehaviour Functions
    void Start()
    {
        //Adding listeners for each of the buttons

        button1.onClick.AddListener(() => { button1Action(); });
        button2.onClick.AddListener(() => { button2Action(); });

        allPlayerUnits.AddRange(GameObject.FindGameObjectsWithTag("PlayerUnit"));

        //Get camera script
        _cameraScript = LevelCamera.GetComponent<LevelCamera>();
        _cameraInit = false;

        //Adds dialogue to the data list variable
        Dialogue.BuildDialogue();

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    void OnEnable()
    {
        Events.instance.AddListener<DialogueEvent>(OnDialogueEvent);
    }

    void OnDisable()
    {
        Events.instance.RemoveListener<DialogueEvent>(OnDialogueEvent);
    }

    void Update()
    {
        if (!_cameraInit)
        {
            //Send initial rotation to camera to ensure walls are hidden on level start
            _cameraScript.RotateCamera(data.cardinalPoints.SOUTH, data.cardinalPoints.EAST);
            _cameraInit = true;
        }

        checkMouse();

        checkSelectionBox();

        selecionCheck();

        generateOrders();

        CameraOrders();

        shuttleCommands();
    }

    //Drawing the selection box to the screen
    private void OnGUI()
    {
        if (startClick != -Vector3.one)
        {
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.DrawTexture(selection, SelectionHighlight);
        }
    }
    #endregion

    #region Event Handelers
    private void OnDialogueEvent(DialogueEvent e)
    {
        GameObject dialogueObject = (GameObject)Instantiate(dialoguePrefab, Vector3.zero, Quaternion.identity);
        DialoguePopup dialogueScript = dialogueObject.GetComponentInChildren<DialoguePopup>();
        dialogueScript.CameraTarget = LevelCamera.transform;
        dialogueScript.ObjectTarget = e.MessageTarget;
        dialogueScript.Text = e.MessageText.DialogueContents;
        dialogueScript.LifeTime = e.MessageLifetime;
    }
    #endregion

    #region Generate Order Functions
    //Generates orders for currently selected units
    void generateOrders()
    {
        if (!haulOrder && !menuOpen)
        {
            foreach (GameObject unit in allPlayerUnits)
            {
                if (Input.GetButtonDown("Interact") && Input.GetButton("Queue") && unit.GetComponent<unit>().isSelected)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, interactablesMask))
                    {
                        if (hit.collider.tag == "Resource")
                        {
                            beginHaulOrder(unit, hit.collider.gameObject);
                        }
                        else if (hit.collider.tag == "door")
                        {
                            interactWithDoor(unit, hit.transform.parent.gameObject);
                        }
                    }
                    else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        addToQueue(hit.point, data.unitAction.STAND, null, unit);
                    }
                }
                else if (Input.GetButtonDown("Interact") && unit.GetComponent<unit>().isSelected)
                {
                    unit.GetComponent<unit>().clearQueue();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, interactablesMask))
                    {
                        if (hit.collider.tag == "Resource")
                        {
                            beginHaulOrder(unit, hit.collider.gameObject);
                        }
                        else if (hit.collider.tag == "door")
                        {
                            interactWithDoor(unit, hit.transform.parent.gameObject);
                        }
                        else if (hit.collider.tag == "command")
                        {
                            action1 = 1;
                            action2 = 2;
                            menuOpen = true;
                            showOrders(true, true, 1);
                        }
                    }
                    else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        unit.GetComponent<unit>().currentDestination = hit.point;
                        addToQueue(hit.point, data.unitAction.STAND, null, unit);
                        currentUnit = null;
                    }
                }
            }
        }
        else
        {
            haulOrderSecondStep();
        }

    }

    //Function that takes a unit and a door object then shows the player the context menu
    //detailing all the interactions available for the door objects
    void interactWithDoor(GameObject unit, GameObject door)
    {
        currentUnit = unit;
        currentDoor = hit.transform.parent.gameObject;
        currentUnit.GetComponent<unit>().door = currentDoor;
        currentUnit.GetComponent<unit>().currentDestination = hit.point;
        currentDestination = hit.point;

        action1 = 1;
        action2 = 1;
        menuOpen = true;
        showOrders(true, true, 1);
    }

    //Function called when the user clicks on a resource object with at least one unit currently selected
    //Begins the 2 step haul order process
    void beginHaulOrder(GameObject unit, GameObject resource)
    {
        currentUnit = unit;
        targetResource = resource;
        haulOrder = true;
    }

    //Contains code for the second step of the haul order process
    void haulOrderSecondStep()
    {
        if (Input.GetButtonDown("Interact") && Input.GetButton("Queue"))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                freeSlots = hit.transform.root.GetComponent<module>().GetFreeSlots();
                if (freeSlots.Count != 0)
                {
                    targetResource.GetComponent<resource>().dropPosition = freeSlots[0].transform.position;
                    addToQueue(Vector3.zero, data.unitAction.PICKUP, targetResource, currentUnit);
                    addToQueue(hit.point, data.unitAction.DROP, null, currentUnit);
                    haulOrder = false;
                }
            }
        }
        else if (Input.GetButtonDown("Interact"))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                freeSlots = hit.transform.root.GetComponent<module>().GetFreeSlots();
                if (freeSlots.Count != 0)
                {
                    targetResource.GetComponent<resource>().dropPosition = freeSlots[0].transform.position;
                    addToQueue(Vector3.zero, data.unitAction.PICKUP, targetResource, currentUnit);
                    addToQueue(hit.point, data.unitAction.DROP, null, currentUnit);
                    haulOrder = false;
                }
            }
        }

        currentUnit.GetComponent<unit>().currentDestination = hit.point;
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

        currentDestination = new Vector3(0, 10, 0);

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
                else
                {
                    unit.GetComponent<unit>().selectionStatus(false);
                }
            }
        }

        if (Input.GetButtonDown("Select") && !buttonClick)
        {
            buttonClick = true;
            //Selects unit if is clicked while underneath mouse cursor
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) && hit.collider.tag == "PlayerUnit")
            {
                showOrders(false, false, 0);
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
                        showOrders(false, false, 0);
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
    //The function that is called when the first button in the context menu is called
    void button1Action()
    {
        if (action1 == 1)
        {
            addToQueue(currentDestination, data.unitAction.STAND, null, currentUnit);
        }
        showOrders(false, false, 0);
        menuOpen = false;
    }
    //The function that is called when the second button in the context menu is called
    //The action2 variable determines what this button does e.g: action2 = 1, gives the current unit the order
    //to open the current door
    void button2Action()
    {
        if (action2 == 1)
        {
            if (!currentDoor.GetComponent<Door>().IsOpen)
            {
                addToQueue(Vector3.zero, data.unitAction.OPENDOOR, currentDoor, currentUnit);
            }
            else
            {
                addToQueue(Vector3.zero, data.unitAction.CLOSEDOOR, currentDoor, currentUnit);
            }
        }
        else if (action2 == 2)
        {
            GameObject.Find("master_control_program").GetComponent<level1_Script>().endLevel();
        }
        showOrders(false, false, 0);
        menuOpen = false;

    }

    //Determines what buttons are shown and what text is displayed based on parameters given and the action1 and action2 variables
    void showOrders(bool button1Visible, bool button2Visible, float showAlpha)
    {
        popup.GetComponent<RectTransform>().position = Input.mousePosition;
        popup.GetComponent<RectTransform>().position += new Vector3(10, 0, 0);

        if (button1Visible)
        {
            if (action1 == 1)
            {
                button1.GetComponentInChildren<Text>().text = "Move";
            }
            button1.GetComponent<CanvasGroup>().alpha = showAlpha;
            if (showAlpha == 0.5f)
            {
                button1.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            else
            {
                button1.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }

        }
        else
        {
            button1.GetComponent<CanvasGroup>().blocksRaycasts = false;
            button1.GetComponent<CanvasGroup>().alpha = 0;
        }
        if (button2Visible)
        {
            if (action2 == 1)
            {
                button2.GetComponentInChildren<Text>().text = "Use Door";
            }
            else if (action2 == 2)
            {
                button2.GetComponentInChildren<Text>().text = "Leave Station";
            }
            button2.GetComponent<CanvasGroup>().alpha = showAlpha;
            button2.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            button2.GetComponent<CanvasGroup>().blocksRaycasts = false;
            button2.GetComponent<CanvasGroup>().alpha = 0;
        }

        if (showAlpha == 0.5f)
        {
            button1.GetComponent<CanvasGroup>().interactable = false;
        }
        else
        {
            button1.GetComponent<CanvasGroup>().interactable = true;
        }

        button2.GetComponent<CanvasGroup>().interactable = button2Visible;
    }

    void checkMouse()
    {
        if (!menuOpen && !haulOrder)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, interactablesMask) && hit.collider.tag == "door")
            {
                foreach (GameObject unit in allPlayerUnits)
                {
                    if (unit.GetComponent<unit>().isSelected)
                    {
                        action1 = 1;
                        action2 = 1;
                        showOrders(true, true, 0.5f);
                    }
                }

            }
            else
            {
                showOrders(false, false, 0);
            }
        }

        if (haulOrder)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) && hit.transform.root.tag == "module" && glowClone == null && thisModule == null)
            {
                thisModule = hit.transform.root.gameObject;

                if (thisModule.GetComponent<module>().GetFreeSlots().Count != 0)
                {
                    glowClone = (GameObject)GameObject.Instantiate(moduleSelectionPrefab);
                    glowClone.transform.parent = hit.transform.root;
                    glowClone.transform.localPosition = new Vector3(0, 1, 0);
                }
                else
                {
                    glowClone = (GameObject)GameObject.Instantiate(moduleSelectionPrefab);
                    glowClone.transform.parent = hit.transform.root;
                    glowClone.transform.localPosition = new Vector3(0, 1, 0);
                    glowClone.GetComponent<ParticleSystem>().startColor = new Color(1, 0, 0, 1);
                }

            }
            else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) && glowClone != null && hit.transform.root.gameObject != thisModule)
            {
                GameObject.Destroy(glowClone);
                glowClone = null;
                thisModule = null;
            }
            else if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                GameObject.Destroy(glowClone);
                glowClone = null;
                thisModule = null;
            }
        }
        else if (glowClone != null)
        {
            GameObject.Destroy(glowClone);
            glowClone = null;
            thisModule = null;
        }
    }
    #endregion

    #region Camera Orders
    private void CameraOrders()
    {
        #region Camera Pan
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _cameraScript.PanCamera(data.cardinalPoints.NORTH);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _cameraScript.PanCamera(data.cardinalPoints.SOUTH);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _cameraScript.PanCamera(data.cardinalPoints.EAST);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _cameraScript.PanCamera(data.cardinalPoints.WEST);
        }
        #endregion

        #region Camera Rotate
        if (Input.GetKeyDown((KeyCode.Q)))
        {
            data.cardinalPoints currentRotation = _cameraScript.GetCurrentRotation();
            if (currentRotation == data.cardinalPoints.SOUTH)
            {
                _cameraScript.RotateCamera(data.cardinalPoints.WEST, data.cardinalPoints.SOUTH);
            }
            else if (currentRotation == data.cardinalPoints.WEST)
            {
                _cameraScript.RotateCamera(data.cardinalPoints.NORTH, data.cardinalPoints.WEST);
            }
            else if (currentRotation == data.cardinalPoints.EAST)
            {
                _cameraScript.RotateCamera(data.cardinalPoints.SOUTH, data.cardinalPoints.EAST);
            }
            else if (currentRotation == data.cardinalPoints.NORTH)
            {
                _cameraScript.RotateCamera(data.cardinalPoints.EAST, data.cardinalPoints.NORTH);
            }
        }
        if (Input.GetKeyDown((KeyCode.E)))
        {
            data.cardinalPoints currentRotation = _cameraScript.GetCurrentRotation();
            if (currentRotation == data.cardinalPoints.SOUTH)
            {
                _cameraScript.RotateCamera(data.cardinalPoints.EAST, data.cardinalPoints.NORTH);
            }
            else if (currentRotation == data.cardinalPoints.WEST)
            {
                _cameraScript.RotateCamera(data.cardinalPoints.SOUTH, data.cardinalPoints.EAST);
            }
            else if (currentRotation == data.cardinalPoints.EAST)
            {
                _cameraScript.RotateCamera(data.cardinalPoints.NORTH, data.cardinalPoints.WEST);
            }
            else if (currentRotation == data.cardinalPoints.NORTH)
            {
                _cameraScript.RotateCamera(data.cardinalPoints.WEST, data.cardinalPoints.SOUTH);
            }
        }
        #endregion

        #region Camera Zoom
        _cameraScript.ZoomCamera(Input.GetAxis("Mouse ScrollWheel"));
        #endregion
    }

    private void shuttleCommands()
    {
        if (Input.GetKey("m"))
        {
            GameObject.FindGameObjectWithTag("shuttle").GetComponent<shuttle>().exitLevel();
        }
    }
    #endregion
}