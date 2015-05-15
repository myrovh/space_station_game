using UnityEngine;
using System.Collections.Generic;

public class unit : MonoBehaviour
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
    public float unitMoveSpeed = 1;
    public float unitStoppingDistance = 1.5f;
    private int baseAvoidance = 89;

    //State Tracking Variables
    public bool isSelected = false;
    private NavMeshAgent agent;

    //Order Queue Variables
    private List<unitOrder> activeOrderQueue = new List<unitOrder>();
    public delegate void MultiDelegate();
    private MultiDelegate passiveOrderQueue;

    //Hauling Variables
    public GameObject target;
    public bool isCarrying = false;
    private GameObject inventory;

    //Door Manipulation Variables
    public GameObject door;

    //Idle Function Variables
    private float rotateSpeed = 3.0f;
    private float timer = 0.0f;
    private Quaternion qto;
    private Vector3 randomPoint;

    //Vision Cone Variables
    Vector3 facingDirection = Vector3.forward;
    float coneLength = 3.0f;
    #endregion

    #region MonoBehaviour Functions
    void OnEnable()
    {
        //Add passive functions here
        passiveOrderQueue += idle;
        passiveOrderQueue += visionCone;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = unitStoppingDistance - 0.75f;
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
    #endregion

    #region Active Queue Manipulation
    //Adds new order to the bottom of the queue
    public void queueOrder(Vector3 moveTo, data.unitAction actAt)
    {
        if (door != null)
        {
            door.GetComponent<Door>().unitUsingDoor = false;
        }

        checkCarrying();

        activeOrderQueue.Add(new unitOrder(moveTo, actAt));
    }

    public void queueOrder(GameObject actAtObject, data.unitAction actAt)
    {
        if (door != null)
        {
            door.GetComponent<Door>().unitUsingDoor = false;
        }

        checkCarrying();

        activeOrderQueue.Add(new unitOrder(actAtObject, actAt));
    }

    //Clears the active order queue
    public void clearQueue()
    {
        activeOrderQueue.Clear();
    }

    void checkCarrying()
    {
        if (isCarrying)
        {
            inventory.GetComponent<resource>().DroppedInFront();
            inventory = null;
            isCarrying = false;
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
        agent.updateRotation = true;
        bool isComplete = false;
        if (activeOrderQueue.Count > 0)
        {
            unitOrder tempOrder = activeOrderQueue[0]; //Get the order on the top of the list
            agent.destination = tempOrder.moveTo; //Tell NavMeshAgent to move to order location
            agent.avoidancePriority = baseAvoidance - activeOrderQueue.Count; //Set the movement priority of this unit based on the number of orders it has queued

            if ((transform.position - agent.destination).magnitude <= unitStoppingDistance)
            {
                switch (tempOrder.actAt)
                {
                    case data.unitAction.STAND:
                        isComplete = true;
                        break;
                    case data.unitAction.PICKUP:
                        pickUp(target);
                        isComplete = true;
                        break;
                    case data.unitAction.DROP:
                        drop();
                        isComplete = true;
                        break;
                    case data.unitAction.OPENDOOR:
                        openDoor(door);
                        isComplete = true;
                        break;
                    case data.unitAction.CLOSEDOOR:
                        closeDoor(door);
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
            agent.avoidancePriority = baseAvoidance + 10;
        }
        return isComplete;
    }

    void pickUp(GameObject newObject)
    {
        inventory = newObject;
        inventory.GetComponent<resource>().PickedUp(this.gameObject);
        isCarrying = true;
        agent.destination = transform.position;
    }

    void drop()
    {
        if (inventory != null)
        {
            inventory.GetComponent<resource>().Dropped();
            inventory = null;
            isCarrying = false;
            agent.destination = transform.position;
        }
    }

    void openDoor(GameObject door)
    {
        if (!door.GetComponent<Door>().isDoorOpen())
        {
            door.GetComponent<Door>().unitUsingDoor = true;
            agent.destination = transform.position;
        }
    }

    void closeDoor(GameObject door)
    {
        door.GetComponent<Door>().unitUsingDoor = true;
        agent.destination = transform.position;
    }
    #endregion

    #region Passive Queue Manipulation
    void idle()
    {
        if (activeOrderQueue.Count == 0)
        {
            agent.updateRotation = false;

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

    void visionCone()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coneLength);
        facingDirection = Vector3.forward;

        foreach (Collider other in hitColliders)
        {

            if (other.tag == "door" && Vector3.Distance(transform.position, other.transform.position) <= unitStoppingDistance + 3.0f)
            {
                door = other.transform.parent.gameObject;
                if (!door.GetComponent<Door>().unitUsingDoor && !door.GetComponent<Door>().isOpen)
                {
                    checkCarrying();
                    queueOrder(other.gameObject, data.unitAction.OPENDOOR);
                }

            }
        }
    }
    #endregion

    #region Selection Functions
    //Call this function and pass a bool to tell the unit if it is selected or not
    //Later these functions will be removed and selection tracking will be handled by the UI
    public void selectionStatus(bool select)
    {
        if (select)
        {
            GetComponent<Renderer>().material.color = Color.red;
            isSelected = true;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
            isSelected = false;
        }
    }
    #endregion
}