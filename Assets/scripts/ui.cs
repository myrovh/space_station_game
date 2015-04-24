using UnityEngine;
using System.Collections;

public class ui : MonoBehaviour {
    //reference to a test unit
	public GameObject controlUnit;

    //Test for input data (gives 2 orders to control unit)
	void Start () {
		data.unitAction tempAction = data.unitAction.STAND; 
		Vector3 tempLocation = new Vector3 (3, 0, 3);

		data.unitOrder tempOrder = new data.unitOrder (tempLocation, tempAction);

		controlUnit.GetComponent<unit> ().queueOrder(tempOrder);


        data.unitAction tempAction2 = data.unitAction.STAND;
        Vector3 tempLocation2 = new Vector3(-3, 0, -3);

        data.unitOrder tempOrder2 = new data.unitOrder(tempLocation2, tempAction2);

        controlUnit.GetComponent<unit>().queueOrder(tempOrder2);
	}
}
