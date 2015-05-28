using UnityEngine;
using System.Collections;

public class Surface : MonoBehaviour {
    private MeshRenderer _renderer;
    private Rigidbody _rigidbody;
    private data.cardinalPoints cardinalLocation;

    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        if (_renderer == null)
        {
            Destroy(this);
        }
        string location = transform.parent.parent.name;
        if (location == "west")
        {
            cardinalLocation = data.cardinalPoints.WEST;
        }
        else if (location == "north")
        {
            cardinalLocation = data.cardinalPoints.NORTH;
        }
        else if (location == "south")
        {
            cardinalLocation = data.cardinalPoints.SOUTH;
        }
        else if (location == "east")
        {
            cardinalLocation = data.cardinalPoints.EAST;
        }
        else if (location == "uppr")
        {
            cardinalLocation = data.cardinalPoints.UPPR;
        }
        else if (location == "down")
        {
            cardinalLocation = data.cardinalPoints.DOWN;
        }
    }

	void OnEnable () {
        Events.instance.AddListener<CameraChange>(OnCameraChange);
	}
	
	void OnDisable () {
        Events.instance.RemoveListener<CameraChange>(OnCameraChange);
	}

    void OnCameraChange(CameraChange e) {
        if (_rigidbody.isKinematic)
        {
            if (cardinalLocation == data.cardinalPoints.UPPR)
            {
                _renderer.enabled = false;
                gameObject.layer = 2;
            }
            else if (cardinalLocation == e.LeftEdge || cardinalLocation == e.RightEdge)
            {
                _renderer.enabled = false;
                gameObject.layer = 2;
            }
            else
            {
                _renderer.enabled = true;
                gameObject.layer = 0;
            }
        }
        else
        {
            _renderer.enabled = true;
        }
    }
}
