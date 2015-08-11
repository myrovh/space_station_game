using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class unitController : MonoBehaviour 
{
    #region Data Classes
    //Use this data container to give orders to units
    private class unitOrder
    {
        public Vector3 moveTo;
        public data.unitAction actAt;
        public GameObject actAtObject;

        public unitOrder(Vector3 moveTo, data.unitAction actAt)
        {
            this.moveTo = moveTo;
            this.actAt = actAt;
            this.actAtObject = null;
        }

        public unitOrder(GameObject actAtObject, data.unitAction actAt)
        {
            this.moveTo = actAtObject.transform.position;
            this.actAt = actAt;
            this.actAtObject = actAtObject;
        }


    }
    #endregion

    #region Variables
    //Setting Variables
    private float unitMoveSpeed = 1;
    private float unitStoppingDistance = 1.5f;
    private int baseAvoidance = 89;
    public Vector3 currentDestination = Vector3.zero;
    //public Dictionary<String, Component> components;
    public unitController[] allComponents;
    public List<string> componentStrings = new List<string>();
    private string orderDescription;

    //State Tracking Variables
    public bool isSelected = false;
    private NavMeshAgent _agent;
    public GameObject selectionGlowPrefab = null;
    private GameObject glowClone = null;

    //Order Queue Variables
    private List<unitOrder> activeOrderQueue = new List<unitOrder>();
    public delegate void MultiDelegate();
    private MultiDelegate passiveOrderQueue;
    public GameObject target;
    public GameObject door;

    //Idle Function Variables
    private float rotateSpeed = 3.0f;
    private float timer = 0.0f;
    private Quaternion qto;
    private Vector3 randomPoint;
    private bool willIdle = true;
    #endregion

    #region MonoBehaviour Functions
    void OnEnable()
    {
        //Add passive functions here
        passiveOrderQueue += idle;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = unitStoppingDistance - 0.75f;

    }

    void getComponents()
    {
        allComponents = this.GetComponents<unitController>();
    }
    
    void getComponentString()
    {
        foreach (unitController component in allComponents)
        {
            componentStrings.Add(component.getDescription);
        }
    }

    string getDescription()
    {
        return orderDescription;
    }

    void Update()
    {
        //Executes any asigned passive orders
        if (passiveOrderQueue != null)
        {
            passiveOrderQueue();
        }

        //Executes the first order on the active order queue
        currentOrder();
    }

    void OnDisable()
    {
        //Clears the passive order queue
        passiveOrderQueue = null;
    }

    //Checks if the unit was selected by a click and sets the selection status to true
    void OnMouseUp()
    {
        selectionStatus(true);
    }
    #endregion

    #region Active Queue Manipulation
    //Adds new order to the bottom of the queue when given a vector3 and a unitAction
    public void queueOrder(Vector3 moveTo, data.unitAction actAt)
    {
        activeOrderQueue.Add(new unitOrder(moveTo, actAt));
    }
    //Adds new order to the bottom of the queue when given a GameObject and a unitAction
    public void queueOrder(GameObject actAtObject, data.unitAction actAt)
    {
        activeOrderQueue.Add(new unitOrder(actAtObject, actAt));
    }

    //Clears the active order queue
    public void clearQueue()
    {
        activeOrderQueue.Clear();
    }
    #endregion

    #region Passive Queue Manipulation
    //Idle function which gets a random point around the object within a certain radius
    //every 2 seconds and rotates the unit to face that direction and moves them slightly towards that point
    void idle()
    {
        if (willIdle)
        {
            if (activeOrderQueue.Count == 0)
            {
                _agent.updateRotation = false;

                timer += Time.deltaTime;

                if (timer > 2)
                {
                    qto = Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0));
                    randomPoint = Random.insideUnitSphere * 0.001f;
                    timer = 0.0f;
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, qto, Time.deltaTime * rotateSpeed);
                transform.Translate(randomPoint);
            }
        }
    }

    public IEnumerator wait(GameObject other)
    {
        queueOrder(other.gameObject, data.unitAction.OPENDOOR);
        willIdle = false;
        yield return new WaitForSeconds(6);
        queueOrder(currentDestination, data.unitAction.STAND);
        willIdle = true;
    }
    #endregion

    #region Selection Functions
    //Call this function and pass a bool to tell the unit if it is selected or not
    //Later these functions will be removed and selection tracking will be handled by the UI
    public void selectionStatus(bool select)
    {
        if (select && glowClone == null)
        {
            glowClone = (GameObject)GameObject.Instantiate(selectionGlowPrefab);
            glowClone.transform.parent = transform;
            glowClone.transform.localPosition = new Vector3(0, -GetComponent<MeshFilter>().mesh.bounds.extents.y, 0);
            isSelected = true;
        }
        else if (!select && glowClone != null)
        {
            GameObject.Destroy(glowClone);
            glowClone = null;
            isSelected = false;
        }
    }

    #endregion

    #region Order Execution
    //If current order is completed delete current order from queue
    void currentOrder()
    {
        if (executeOrder())
        {
            activeOrderQueue.RemoveAt(0);
        }
    }

    //returns true when current order is completed
    bool executeOrder()
    {
        _agent.updateRotation = true;
        bool isComplete = false;
        if (activeOrderQueue.Count > 0)
        {
            unitOrder tempOrder = activeOrderQueue[0]; //Get the order on the top of the list
            _agent.destination = tempOrder.moveTo; //Tell NavMeshAgent to move to order location
            _agent.avoidancePriority = baseAvoidance - activeOrderQueue.Count; //Set the movement priority of this unit based on the number of orders it has queued

            if ((transform.position - _agent.destination).magnitude <= unitStoppingDistance)
            {
                switch (tempOrder.actAt)
                {
                    case data.unitAction.STAND:
                        isComplete = true;
                        break;
                    case data.unitAction.PICKUP:
                        transform.GetComponent<unitCarry>().pickUp(target);
                        isComplete = true;
                        break;
                    case data.unitAction.DROP:
                        transform.GetComponent<unitCarry>().drop();
                        isComplete = true;
                        break;
                    case data.unitAction.OPENDOOR:
                        transform.GetComponent<unitDoorInteract>().openDoor(door);
                        isComplete = true;
                        break;
                    case data.unitAction.CLOSEDOOR:
                        transform.GetComponent<unitDoorInteract>().closeDoor(door);
                        isComplete = true;
                        break;
                    default:
                        isComplete = true;
                        break;
                }
            }
        }

        //If the order has been compleated then increase the avoidancePriority so this unit will move out of the way of units with orders
        if (activeOrderQueue.Count == 0)
        {
            _agent.avoidancePriority = baseAvoidance + 10;
        }
        return isComplete;
    }
    #endregion
}
