using System.Collections;
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
    private float unitMoveSpeed = 1;
    private float unitStoppingDistance = 1.5f;
    private int baseAvoidance = 89;
    public float health = 100;

    //State Tracking Variables
    public bool isSelected = false;
    private NavMeshAgent _agent;
    public GameObject selectionGlowPrefab = null;
    private GameObject glowClone = null;

    //Order Queue Variables
    private List<unitOrder> activeOrderQueue = new List<unitOrder>();
    public delegate void MultiDelegate();
    private MultiDelegate passiveOrderQueue;

    //Hauling Variables
    //accept reference to module
    public GameObject target;
    public bool isCarrying = false;
    private GameObject inventory;
    public Vector3 currentDestination = Vector3.zero;

    //Door Manipulation Variables
    public GameObject door;

    //Idle Function Variables
    private float rotateSpeed = 3.0f;
    private float timer = 0.0f;
    private Quaternion qto;
    private Vector3 randomPoint;
    private bool willIdle = true;

    //Vision Cone Variables

    //Sound Variables
    private AudioSource _audioSource;

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
        _audioSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = unitStoppingDistance - 0.75f;

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
        if (door != null)
        {
            door.GetComponent<Door>().UnitUsingDoor = false;
        }

        checkCarrying();

        activeOrderQueue.Add(new unitOrder(moveTo, actAt));
        SpeakDialogue(Dialogue.GetRandomDialogueType(Dialogue.DialogueType.MOVEORDER));
    }
    //Adds new order to the bottom of the queue when given a GameObject and a unitAction
    public void queueOrder(GameObject actAtObject, data.unitAction actAt)
    {
        if (door != null)
        {
            door.GetComponent<Door>().UnitUsingDoor = false;
        }

        checkCarrying();

        activeOrderQueue.Add(new unitOrder(actAtObject, actAt));
    }

    //Clears the active order queue
    public void clearQueue()
    {
        activeOrderQueue.Clear();
    }
    //Called when we want the unit to drop what they are carrying in front of them
    //For example when their haul order was interupted
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
            _agent.avoidancePriority = baseAvoidance + 10;
        }
        return isComplete;
    }
    //Function to make unit pick up the object passed as a parameter
    void pickUp(GameObject newObject)
    {
        inventory = newObject;
        inventory.GetComponent<resource>().PickedUp(this.gameObject);
        isCarrying = true;
        _agent.destination = transform.position;
    }
    //Function to make the unit drop the resource object they are carrying
    //If the unit has not reached its destination i.e: the path is blocked
    //it will drop the resource object in front of it
    void drop()
    {
        if (inventory != null)
        {
            if ((transform.position - currentDestination).magnitude <= unitStoppingDistance + 1)
            {
                inventory.GetComponent<resource>().Dropped();
                inventory = null;
                isCarrying = false;
                _agent.destination = transform.position;
            }
            else
            {
                checkCarrying();
            }
        }
    }
    //Function that tells the unit to open the door that was passed as a parameter
    void openDoor(GameObject door)
    {
        if (!door.GetComponent<Door>().IsDoorOpen())
        {
            door.GetComponent<Door>().UnitUsingDoor = true;
            _agent.destination = transform.position;
        }

    }
    //Function that tells the unit to close the door that was passed as a parameter
    void closeDoor(GameObject door)
    {
        door.GetComponent<Door>().UnitUsingDoor = true;
        _agent.destination = transform.position;
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
    //This is where the new vision cone code will most likely be
    private void visionCone()
    {

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
    //Simple function which makes the units health lower by 1 each second
    //Used for event system testing
    public void takeDamage()
    {
        health = health - 1 * Time.deltaTime;
    }
    #endregion

    #region Dialogue Functions
    public void RaiseDialogue(DialogueText text)
    {
        //TODO add test to make sure that unit will not raise a new dialogue when already talking
        _audioSource.clip = Dialogue.GetDialogueAudio(text);
        if (_audioSource.clip != null)
        {
            Events.instance.Raise(new DialogueEvent(transform, text, _audioSource.clip.length));
        }
    }

    public void SpeakDialogue(DialogueText text)
    {
        if (!_audioSource.isPlaying)
        {
            RaiseDialogue(text);
            if (_audioSource.clip != null)
            {
                StartCoroutine(PlayAudio());
            }  
        }
    }

    IEnumerator PlayAudio()
    {
        _audioSource.Play();
        yield return new WaitForSeconds(_audioSource.clip.length);
        _audioSource.Stop();
    }
    #endregion
}