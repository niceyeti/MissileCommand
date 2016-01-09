using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace MissileCommand.Input
{
  /// <summary>
  /// This is just a wrapper to genericize a mouse device that may be either
  /// the real mouse, or an eye tracker.
  /// </summary>
  public class MouseDevice : IMouseDevice
  {
    public MouseDevice()
    { }

    public MouseState GetMouseState()
    {
      return Mouse.GetState();
    }
  }
}
