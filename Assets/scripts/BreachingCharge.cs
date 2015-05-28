using UnityEngine;
using System.Collections.Generic;

public class BreachingCharge : MonoBehaviour {
    private List<Rigidbody> _objectsInRange = new List<Rigidbody>();
    public float ExplosionRange = 5.0f;
    public float ExplosionForce = 5.0f;
    public float Lifetime = 5.0f;


	// Use this for initialization
	void Start ()
	{
	    var thisCollider = gameObject.AddComponent<SphereCollider>();
	    thisCollider.radius = ExplosionRange;
	    thisCollider.isTrigger = true;
	}

    private void OnTriggerEnter(Collider otherCollider)
    {
        Rigidbody tempCollider = otherCollider.GetComponent<Rigidbody>();
        if (tempCollider != null)
        {
            _objectsInRange.Add(tempCollider);
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        Rigidbody tempCollider = otherCollider.GetComponent<Rigidbody>();
        if (tempCollider != null)
        {
            _objectsInRange.Remove(tempCollider);
        }
    }

    private void Update()
    {
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0)
        {
            ExplosionStart();
            Destroy(gameObject);
        }
    }

    private void ExplosionStart()
    {
        foreach (var a in _objectsInRange)
        {
            a.isKinematic = false;
            a.AddExplosionForce(ExplosionForce, gameObject.transform.position, ExplosionRange);
        }
    }
}