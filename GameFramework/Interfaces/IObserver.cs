/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand.EventSystem
{
  //abstract class for observers to implement. for the most part, observers will be managers
  // of some kind (other sub-systems) which manage logic for object creation/destruction
  public interface IObserver
  {
    void Notify(EventPacket newEvent);
  }
}
