/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using MissileCommand.Kinematics;


namespace MissileCommand
{
  public delegate void MouseInputHandler(MouseState mouseState, Position mousePosition);
  public delegate void KeyboardInputHandler(KeyboardState keyboardState);

  public interface IGameViewFramework
  {
    IGameSpriteFactory GetIGameSpriteFactory();
    Position GetScreenDimension();
    //callback for sending user input events to the controller
    //These could also be encapsulated in an class: SetInputHandler(UserInputHandler object) where object implements all callbacks.
    void SetMouseInputHandler(MouseInputHandler onMouseEvent);
    void SetKeyboardInputHandler(KeyboardInputHandler onKeyboardEvent);
  }
}
