using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand.EventSystem
{
  /*
  The event bus receives event packets, and distributes them to event subscribers.
   */
  public class EventBus
  {
    Dictionary<EventTypes, HashSet<IObserver>> listenerTable;
    Queue<EventPacket> eventQueue { get; set; }

    public EventBus()
    {
      listenerTable = new Dictionary<EventTypes, HashSet<IObserver>>();
      eventQueue = new Queue<EventPacket>();
    }

    /*
      Subscribers/listeners register with the event system
    */
    public void Subscribe(IObserver listener, EventTypes eventType)
    {
      if (!listenerTable.Keys.Contains(eventType))
      {
        listenerTable[eventType] = new HashSet<IObserver>();
      }
      listenerTable[eventType].Add(listener);
    }

    /// <summary>
    /// Allows clients to subscribe to multiple event types by passing in a list of
    /// types. 
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="eventTypes"></param>
    public void SubscribeMultiple(IObserver listener, List<EventTypes> eventTypes)
    {
      foreach (EventTypes type in eventTypes)
      {
        Subscribe(listener, type);
      }
    }

    // unsubscribe this object from all its events
    public void UnsubscribeAll(IObserver listener)
    {
      foreach (EventTypes key in listenerTable.Keys)
      {
        if (listenerTable[key].Contains(listener))
        {
          listenerTable[key].Remove(listener);
        }
      }
    }

    //unsubscribe this event from event matching eventType
    public void Unsubscribe(IObserver listener, EventTypes eventType)
    {
      if (listenerTable.Keys.Contains(eventType))
      {
        listenerTable[eventType].Remove(listener);
      }
    }

    public void Receive(EventPacket newEvent)
    {
      eventQueue.Enqueue(newEvent);
    }

    void notifyObservers(EventPacket pendingEvent)
    {
      foreach (IObserver observer in listenerTable[pendingEvent.Type])
      {
        observer.Notify(pendingEvent);
      }
    }

    //this is really just an isNullOrEmpty check on the observer list for this key
    public bool HasObservers(EventPacket pendingEvent)
    {
      bool hasObserver = listenerTable[pendingEvent.Type] != null;

      if(listenerTable[pendingEvent.Type] == null)
      {
        hasObserver = false;
      }
      else if (listenerTable[pendingEvent.Type].Count == 0)
      {
        hasObserver = false;
      }

      return hasObserver;
    }

    /*
     Notifies subscribers/observers of a dequeued event.
     */
    public void Process(EventPacket pendingEvent)
    {
      if (listenerTable.Keys.Contains(pendingEvent.Type))
      {
        if (HasObservers(pendingEvent))
        {
          notifyObservers(pendingEvent);
        }
        else
        {
          GameLogger.Instance.Write("WARN event type has no observers: " + pendingEvent.Type.ToString());
        }
      }
      else 
      {
        GameLogger.Instance.Write("ERROR EventSystem.Process listener table key not found: "+pendingEvent.Type.ToString());
      }
    }

    /*
     Empties the event queue, notifying all observers of each dequeued event.
     */
    public void ProcessAll()
    {
      while (eventQueue.Count > 0)
      {
        Process(eventQueue.Dequeue());
      }
    }
  }
}
