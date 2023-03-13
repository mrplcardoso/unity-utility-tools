using System;
using EventObserverReaction = EventHub.EventObserverReaction;

public static class EventHubExtensions
{
	public static void PostEvent(this object poster, string eventName, EventArgs args)
	{
		EventHub.instance.PostEvent(eventName, args);
	}
	public static void PostEvent(this object poster, string eventName)
	{
		EventHub.instance.PostEvent(eventName, EventArgs.Empty);
	}

	public static void AddObserver(this object observer, string eventName, EventObserverReaction eventReaction)
	{
		EventHub.instance.AddObserver(eventName, eventReaction);
	}

	public static void RemoveObserver(this object observer, string eventName, EventObserverReaction eventReaction)
	{
		EventHub.instance.RemoveObserver(eventName, eventReaction);
	}
}
