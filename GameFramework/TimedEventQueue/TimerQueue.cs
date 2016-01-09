using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.EventSystem;


namespace MissileCommand.TimedEventQueue
{
  /*
   TimerQueue is just a proximity-ordered linked list, with the soonest-to-expire
   event at the front. Event-class objects are simply wrapped with a timer value,
   * when the timer expires, the event is processed. Time is just measured in ticks, for now.
   * Since the list is ordered by time value, the timed-events need not maintain absolute
   * time-counters, since they are recursively defined by the preceding timed event.
   * For example, if there are two events in the Q, one with 7 seconds and the other
   * with 5 seconds until expiration, then the first timed event is stored as
   * 5 seconds, the next event as 2 seconds, since 5 + 2 = 7. This makes it so only
   * the topmost event needs to be decremented.
   * 
   * This could have subscribers to expiration events, but won't do that until model settles.
   */
  class TimerQueue
  {
    EventBus _eventBus;
    List<TimedEvent> _Q;

    public TimerQueue(EventBus eventBus)
    {
      _eventBus = eventBus;
      _Q = new List<TimedEvent>();
    }

    /*
     Updates the timer queue per some elapsed time span. All events expiring within the
     timespan (decrementValue) are popped and enqueued into the event system for processing.
     */
    public void Tick(int decrementValue)
    {
      if (_Q.Count > 0)
      {
        _Q[0].Timer_ms -= decrementValue;

        //process expired events
        while (Peek() != null && Peek().Timer_ms <= 0)
        {
          EventPacket newEvent = Pop();
          _eventBus.Receive(newEvent);
        }
      }
    }

    public int Count()
    {
      return _Q.Count;
    }

    //peek at the front item. if no items, returns null.
    public TimedEvent Peek()
    {
      if (_Q.Count > 0)
      {
        return _Q[0];
      }
      return null;
    }

    public void Print()
    {
      foreach (TimedEvent te in _Q)
      {
        Console.Write(" {0}:{1}type ->",te.Timer_ms, te.TimedEventPacket.Type);
      }
      Console.WriteLine("null");
    }

    //Pop front item. Returns null if no event. No checks on timer value, clients can pop() arbitrarily.
    public EventPacket Pop()
    {
      EventPacket expiredEvent = null;

      if (_Q.Count > 0)
      {
        expiredEvent = _Q[0].TimedEventPacket;
        _Q.RemoveAt(0);
      }
      return expiredEvent;
    }

    /// <summary>.
    /// Insert a timer event in the queue. Timer values are
    /// recursively defined by the summation of preceding timer values.
    /// So inserting a timeEvent with a value of 7 seconds may look as follows:
    /// 
    /// Q before:   2 -> 3 -> 1 -> 5 -> ...
    ///
    /// Insert(7):  2 -> 3 -> 1 -> 1 -> 4 -> ...
    ///                            ^ seven inserted here. 1 + summation of preceding timers is 7. 
    /// </summary>
    public void Insert(TimedEvent timedEvent)
    {
      int i = 0;

      Console.WriteLine("insert " + timedEvent.Timer_ms);

      //spool to proximity-based index for this event 
      while (timedEvent.Timer_ms > 0 && i < _Q.Count)
      {
        timedEvent.Timer_ms -= _Q[i].Timer_ms;
        if (timedEvent.Timer_ms > 0)
        {
          i++;
        }
      }

      //insert and decrement right neighbor
      if (i < _Q.Count)
      {
        //There's some pencil work here to adjust the timerVals, and the neighbor.
        timedEvent.Timer_ms += _Q[i].Timer_ms;
        _Q[i].Timer_ms -= timedEvent.Timer_ms;
        _Q.Insert(i, timedEvent);
      }
      //degenerate case: end of list, just insert at the end
      else
      {
        _Q.Add(timedEvent);
      }
      //Console.WriteLine("Inserted event with tmr: "+timedEvent._timer_ms);
    }
  }
}
