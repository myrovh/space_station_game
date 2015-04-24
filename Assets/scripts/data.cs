using UnityEngine;
using System.Collections;

public static class data{
    //Defines unit action upon reaching destination
	public enum unitAction {STAND};

    //Use this data container to give orders to units
	public class unitOrder {
		public Vector3 moveTo; 
		public unitAction actAt;

		public unitOrder(Vector3 moveTo, unitAction actAt) {
			this.moveTo = moveTo;
			this.actAt = actAt;
		}
	}
}