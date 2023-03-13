using System;
using System.Collections.Generic;

/// <summary>
/// Singleton class to handle Pub/Sub Pattern
/// </summary>
public class EventHub
{
	public delegate void EventObserverReaction(EventArgs e);
	/// <summary>
	/// Match an event with a list of observers' reaction
	/// </summary>
	Dictionary<string, List<EventObserverReaction>> eventObservers = 
		new Dictionary<string, List<EventObserverReaction>>();

	public readonly static EventHub instance = new EventHub();
	private EventHub(){ }

	public void AddObserver(string eventName, EventObserverReaction eventReaction)
	{
		if(string.IsNullOrEmpty(eventName))
		{ PrintConsole.Error("Empty event name"); return; }
		if(eventReaction == null)
		{ PrintConsole.Error("Null event reaction"); return; }

		if(!eventObservers.ContainsKey(eventName))
		{ eventObservers.Add(eventName, new List<EventObserverReaction>()); }
		else if (eventObservers[eventName].Contains(eventReaction)) 
		{ PrintConsole.Warning("Already observing this event"); return; }
		
		eventObservers[eventName].Add(eventReaction);
	}

	public void PostEvent(string eventName, EventArgs e)
	{
		if (string.IsNullOrEmpty(eventName))
		{ PrintConsole.Warning("Empty event name"); return; }
		if (!eventObservers.ContainsKey(eventName))
		{ PrintConsole.Warning("No observers to react"); return; }

		List<EventObserverReaction> l = eventObservers[eventName];
		for(int i = 0; i < l.Count; ++i)
		{
			l[i](e);
		}
	}

	public void RemoveObserver(string eventName, EventObserverReaction eventReaction)
	{
		if (string.IsNullOrEmpty(eventName))
		{ PrintConsole.Error("Empty event name"); return; }
		if (eventReaction == null)
		{ PrintConsole.Error("Null event reaction"); return; }

		if (!eventObservers.ContainsKey(eventName))
		{ PrintConsole.Warning("No event/observers found"); return; }
		if(!eventObservers[eventName].Contains(eventReaction))
		{ PrintConsole.Warning("No event reaction found"); return; }

		eventObservers[eventName].Remove(eventReaction);
		if(eventObservers[eventName].Count == 0)
		{ RemoveHub(eventName); }
	}

	/// <summary>
	/// Remove an event and it's list of observers
	/// </summary>
	/// <param name="eventName"></param>
	void RemoveHub(string eventName)
	{
		if (string.IsNullOrEmpty(eventName))
		{ PrintConsole.Error("Empty event name"); return; }
		if (!eventObservers.ContainsKey(eventName))
		{ PrintConsole.Warning("No event/observers found"); return; }

		eventObservers[eventName].Clear();
		eventObservers.Remove(eventName);
	}
}
