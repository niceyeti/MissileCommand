/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand.EventSystem
{
  /*
   The intent here is to approximate a c/c++ void* event implementation.
   Events could well be some abstract/base class, this is intentionally more
   packet struct in nature.
   */
  public class EventPacket
  {
    public EventPacket(EventTypes type, byte[] data)
    {
      Type = type;
      Data = data;
    }

    public EventTypes Type;
    public Byte[] Data;
  }
}
