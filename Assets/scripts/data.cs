using UnityEngine;
using System.Collections;

public static class data
{
    //Defines unit action upon reaching destination
    public enum unitAction { STAND, PICKUP, DROP, OPENDOOR, CLOSEDOOR };
    //Defines resource types for data collection, might be able to change this to use multitags
    public enum resourceType { AMMO, REACTIONMASS };
    //Defines possible interactions for objects in the game
    public enum interactions { HAUL };
}