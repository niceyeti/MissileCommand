/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand.EventSystem
{
  // This probably is optional, but is a bidirectional implementation of the Observer pattern.
  // Observers for whom Notify() has been called may then call GetState() on the Subject (the observed event).
  // see http://www.hitmaroc.net/1076065-8938-how-avoid-downcast.html for commentary on downcasting events
  interface ISubject
  {
    void GetState();
  }
}
