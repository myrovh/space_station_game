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
    public float unitMoveSpeed = 1;
    public float unitStoppingDistance = 1.5f;
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
    Vector3 facingDirection;
    float coneLength = 1.0f;
    public Collider[] hitColliders;

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

    void OnMouseUp()
    {
        selectionStatus(true);
    }
    #endregion

    #region Active Queue Manipulation
    //Adds new order to the bottom of the queue
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

    void pickUp(GameObject newObject)
    {
        inventory = newObject;
        inventory.GetComponent<resource>().PickedUp(this.gameObject);
        isCarrying = true;
        _agent.destination = transform.position;
    }

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

    void openDoor(GameObject door)
    {
        if (!door.GetComponent<Door>().IsDoorOpen())
        {
            door.GetComponent<Door>().UnitUsingDoor = true;
            _agent.destination = transform.position;
        }

    }

    void closeDoor(GameObject door)
    {
        door.GetComponent<Door>().UnitUsingDoor = true;
        _agent.destination = transform.position;
    }
    #endregion

    #region Passive Queue Manipulation
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

    void visionCone()
    {
        hitColliders = Physics.OverlapSphere(transform.position, coneLength);
        facingDirection = Vector3.forward;
        if ((transform.position - currentDestination).magnitude > unitStoppingDistance)
        {
            foreach (Collider other in hitColliders)
            {
                float angle = Vector3.Angle(other.transform.position, facingDirection);
            if (angle < 45.0f)

                if (angle > 45.0f)
            if (other.tag == "door" && Vector3.Distance(transform.position, other.transform.position) <= unitStoppingDistance + 3.0f)
                if (activeOrderQueue.Count > 0)
                {
                    if (other.tag == "door" && Vector3.Distance(transform.position, other.transform.position) < unitStoppingDistance + 7.0f)
                {
                    if (other.tag == "door" && Vector3.Distance(transform.position, other.transform.position) <= unitStoppingDistance + 3.0f && currentDestination != null)
                    {
                        door = other.transform.parent.gameObject;
                        
                        if (!door.GetComponent<Door>().UnitUsingDoor && !door.GetComponent<Door>().IsOpen)
                        {
                            _agent.destination = other.transform.position;
                            StartCoroutine(wait(other.gameObject));
                            
                        }
                    }
                }
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

    public void takeDamage()
    {
        health = health - 1 * Time.deltaTime;
    }
    #endregion

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
}