using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.EventSystem;

namespace MissileCommand.TimedEventQueue
{
  class TimedEvent
  {
    public int Timer_ms { get; set; }
    public EventPacket TimedEventPacket { get; set; }
    public TimedEvent(int timerVal_ms, EventPacket newEvent)
    {
      Timer_ms = timerVal_ms;
      TimedEventPacket = newEvent;
    }
  }
}
