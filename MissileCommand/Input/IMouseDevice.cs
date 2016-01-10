/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace MissileCommand.Input
{
  interface IMouseDevice
  {
    MouseState GetMouseState();
  }
}
