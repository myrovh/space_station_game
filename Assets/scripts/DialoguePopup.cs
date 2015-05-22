using UnityEngine;

public class DialoguePopup : MonoBehaviour
{
    public float LifeTime;
    public string Text;
    public Transform CameraTarget;
    public Transform ObjectTarget;
    public Vector3 Displacement = new Vector3(0, 2.0f, 0);
    private TextMesh _textDisplay;
    private bool initRenderer = false;

    void Start()
    {
        _textDisplay = gameObject.GetComponent<TextMesh>();
    }
	
	void Update ()
	{
        _textDisplay.text = Text;
        transform.LookAt(CameraTarget);
        transform.parent.position = ObjectTarget.position + Displacement;
	    LifeTime = LifeTime - Time.deltaTime;
        if (!initRenderer)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            initRenderer = true;
        }
	    if (LifeTime < 0)
	    {
	        Destroy(transform.parent.gameObject);
	    }
	}
}