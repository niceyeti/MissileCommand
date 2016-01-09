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
  class MouseEvent
  {
    public bool IsLeftClick;
    public Position EventPosition;

    public MouseEvent(MouseState mouseState, Position mousePosition)
    {
      IsLeftClick = mouseState.LeftButton == ButtonState.Pressed;
      EventPosition = new Position(mousePosition.X,mousePosition.Y);
    }
  }
}
