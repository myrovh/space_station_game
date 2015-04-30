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

        public unitOrder(Vector3 moveTo, data.unitAction actAt)
        {
            this.moveTo = moveTo;
            this.actAt = actAt;
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
    delegate void MultiDelegate();
    private MultiDelegate passiveOrderQueue;

    //Hauling Variables
    public GameObject target;
    private GameObject inventory;
    public bool isCarrying = false;
    #endregion

    #region MonoBehaviour Functions
    void OnEnable()
    {
        //Add passive functions here
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
        activeOrderQueue.Add(new unitOrder(moveTo, actAt));
    }

    //Clears the active order queue
    public void clearQueue()
    {
        activeOrderQueue.Clear();
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
        inventory.GetComponent<resource>().PickedUp(transform.gameObject);
        isCarrying = true;
        agent.destination = transform.position;
    }

    void drop()
    {
        inventory.GetComponent<resource>().Dropped();
        inventory = null;
        isCarrying = false;
        agent.destination = transform.position;
    }
    #endregion

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
}