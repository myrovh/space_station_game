using UnityEngine;
using System.Collections;

public class eventManager : MonoBehaviour 
{

    public delegate void moduleEventHandler();

    public static event moduleEventHandler depressurize;

/*    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Toggle Pressure"))
        {
            if (depressurize != null)
                depressurize();
        }
    }
*/

    public static void depressurizeModule()
    {
        if (depressurize != null)
            depressurize();
    }
}
