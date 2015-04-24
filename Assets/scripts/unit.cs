using UnityEngine;
using System.Collections;

public class unit : MonoBehaviour {

	public float unitMoveSpeed = 1;
	delegate void MultiDelegate();

	MultiDelegate passiveOrderQueue;
	
	void Start () {

	}

	void OnEnable(){
		passiveOrderQueue = turnUnit;
	}

	void OnDisable(){
		passiveOrderQueue = null;
	}

	void turnUnit(){
		transform.Rotate ((Vector3.right * unitMoveSpeed) * Time.deltaTime);
	}
	
	void Update () {
		if (passiveOrderQueue != null) {
			passiveOrderQueue ();
		}
	}
}
