/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Xna.Framework.Input;
using MissileCommand.Kinematics;

namespace MissileCommand.EventSystem.Events
{
  [Serializable]
  class KeyboardEvent
  {
    public bool FixThisParameter;

    public KeyboardEvent(KeyboardState keyboardState)
    {
      //could map the pressed keys into some serializable string
      FixThisParameter = keyboardState.IsKeyDown(Keys.A);
    }
  }
}
