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
    public float unitStoppingDistance = 3;

    //State Tracking Variables
    public bool isSelected = false;
    public bool selectedByClick = false;
    NavMeshAgent agent;

    //Order Queue Variables
    private List<unitOrder> activeOrderQueue = new List<unitOrder>();
    delegate void MultiDelegate();
    MultiDelegate passiveOrderQueue;
    #endregion

    #region MonoBehaviour Functions
    void OnEnable()
    {
        //Add passive functions here
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
            unitOrder tempOrder = activeOrderQueue[0];

            agent.destination = tempOrder.moveTo;

            if ((transform.position - agent.destination).magnitude <= unitStoppingDistance)
            {
                switch (tempOrder.actAt)
                {
                    case data.unitAction.STAND:
                        isComplete = true;
                        break;
                    default:
                        isComplete = true;
                        break;
                }
            }
        }
        return isComplete;
    }

    //Call this function and pass a bool to tell the unit if it is selected or not
    //Later these functions will be removed and selection tracking will be handled by the UI
    public void SelectionStatus(bool select)
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