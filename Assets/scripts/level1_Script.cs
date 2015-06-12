using UnityEngine;
using System.Collections;

public class level1_Script : MonoBehaviour
{

    public GameObject shuttle;
    public GameObject unit1;
    public GameObject unit2;

    void OnEnable()
    {
        Events.instance.AddListener<ShuttleLocation>(OnShuttleLocationEvent);
    }

    void OnDisable()
    {
        Events.instance.RemoveListener<ShuttleLocation>(OnShuttleLocationEvent);
    }

    private void OnShuttleLocationEvent(ShuttleLocation e)
    {
        if (e.locationName == "shuttleMoving")
        {
            GameObject.Find("camera").GetComponent<LevelCamera>().cameraEnabled = false;
        }
        else if (e.locationName == "dockPosition")
        {
            GameObject.Find("camera").GetComponent<LevelCamera>().cameraEnabled = true;
        }
        else if (e.locationName == "endPosition")
        {
            Application.LoadLevel("level_2");
        }
    }

    void Start()
    {
        shuttle.GetComponent<shuttle>().enterLevel();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(WaitForShuttle());
    }

    public void endLevel()
    {
        shuttle.GetComponent<shuttle>().exitLevel();
        unit1.GetComponent<MeshRenderer>().enabled = false;
        unit1.GetComponent<Light>().enabled = false;
        unit2.GetComponent<MeshRenderer>().enabled = false;
        unit2.GetComponent<Light>().enabled = false;
    }

    IEnumerator WaitForShuttle()
    {
        yield return new WaitForSeconds(3.5f);
        unit1.GetComponent<MeshRenderer>().enabled = true;
        unit1.GetComponent<Light>().enabled = true;
        unit2.GetComponent<MeshRenderer>().enabled = true;
        unit2.GetComponent<Light>().enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
