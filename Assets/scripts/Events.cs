using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
}

public class CameraChange : GameEvent
{
    public data.cardinalPoints LeftEdge;
    public data.cardinalPoints RightEdge;

    public CameraChange(data.cardinalPoints leftEdge, data.cardinalPoints rightEdge)
    {
        LeftEdge = leftEdge;
        RightEdge = rightEdge;
    }
}

public class DialogueEvent : GameEvent
{
    public DialogueText MessageText;
    public Transform MessageTarget;
    public float MessageLifetime;

    public DialogueEvent(Transform target, DialogueText text, float lifetime)
    {
        MessageText = text;
        MessageTarget = target;
        MessageLifetime = lifetime;
    }
}

public class ShuttleLocation : GameEvent
{
    public string locationName;
    public Vector3 location;
    public ShuttleLocation(string locationName, Vector3 location)
    {
        this.locationName = locationName;
        this.location = location;
    }

}

 public class Events
{
	static Events instanceInternal = null;
	public static Events instance
	{
		get
		{
			if (instanceInternal == null)
			{
				instanceInternal = new Events();
			}
			
			return instanceInternal;
		}
	}
	
	public delegate void EventDelegate<T> (T e) where T : GameEvent;
	private delegate void EventDelegate (GameEvent e);
	
	private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
	private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();
	
	public void AddListener<T> (EventDelegate<T> del) where T : GameEvent
	{	
		// Early-out if we've already registered this delegate
		if (delegateLookup.ContainsKey(del))
			return;
		
		// Create a new non-generic delegate which calls our generic one.
		// This is the delegate we actually invoke.
		EventDelegate internalDelegate = (e) => del((T)e);
		delegateLookup[del] = internalDelegate;
		
		EventDelegate tempDel;
		if (delegates.TryGetValue(typeof(T), out tempDel))
		{
			delegates[typeof(T)] = tempDel += internalDelegate; 
		}
		else
		{
			delegates[typeof(T)] = internalDelegate;
		}
	}
	
	public void RemoveListener<T> (EventDelegate<T> del) where T : GameEvent
	{
		EventDelegate internalDelegate;
		if (delegateLookup.TryGetValue(del, out internalDelegate))
		{
			EventDelegate tempDel;
			if (delegates.TryGetValue(typeof(T), out tempDel))
			{
				tempDel -= internalDelegate;
				if (tempDel == null)
				{
					delegates.Remove(typeof(T));
				}
				else
				{
					delegates[typeof(T)] = tempDel;
				}
			}
			
			delegateLookup.Remove(del);
		}
	}
	
	public void Raise (GameEvent e)
	{
		EventDelegate del;
		if (delegates.TryGetValue(e.GetType(), out del))
		{
			del.Invoke(e);
		}
	}
}