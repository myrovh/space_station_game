using UnityEngine;
using System.Collections;

public class eventManager : MonoBehaviour 
{

    public delegate void moduleEventHandler();

    public static event moduleEventHandler depressurize;

    public static void depressurizeModule()
    {
        if (depressurize != null)
            depressurize();
    }
}
