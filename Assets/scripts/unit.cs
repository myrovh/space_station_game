using UnityEngine;
using System.Collections.Generic;

public class unit : MonoBehaviour {

	NavMeshAgent agent;
	public float unitMoveSpeed = 1;
    public float unitStoppingDistance = 3;

	public List<data.unitOrder> activeOrderQueue = new List<data.unitOrder>();

	delegate void MultiDelegate();
	MultiDelegate passiveOrderQueue;
	
	void Start () {
		agent = GetComponent<NavMeshAgent>();
	}

    //Adds new order to the bottom of the queue
	public void queueOrder(data.unitOrder newOrder){
		activeOrderQueue.Add (newOrder);
	}

    //If current order is completed delete current order from queue
	void currentOrder(){
		if (executeOrder ()) {
			activeOrderQueue.RemoveAt(0);
		}
	}

    //returns true when current order is completed
	bool executeOrder(){
		bool isComplete = false;
        if (activeOrderQueue.Count > 0)
        {
            data.unitOrder tempOrder = activeOrderQueue[0];

            agent.destination = tempOrder.moveTo;

            if ((transform.position - agent.destination).magnitude <= unitStoppingDistance )
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

	void OnEnable(){
		//Add passive functions here
	}

	void OnDisable(){
		passiveOrderQueue = null;
	}

	void Update () {
        //Executes any asigned passive orders
		if (passiveOrderQueue != null) {
			passiveOrderQueue ();
		}
        //Executes the first order on the active order queue
		currentOrder ();
	}
}
