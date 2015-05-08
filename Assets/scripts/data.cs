using UnityEngine;
using System.Collections;

public static class data
{
    //Defines unit action upon reaching destination
    public enum unitAction { STAND, PICKUP, DROP };
    //Defines resource types for data collection, might be able to change this to use multitags
    public enum resourceType { AMMO, REACTIONMASS };
}