/* Copyright (c) 2015-2016 Jesse Waite */
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MissileCommand.Kinematics;
using MissileCommand.Interfaces;

namespace MissileCommand
{
  public delegate void MouseInputHandler(MouseState mouseState, Position mousePosition);
  public delegate void KeyboardInputHandler(KeyboardState keyboardState);
  public delegate void LevelCompletionHandler(int remainingCities, int remainingAmmo, int score);
  public delegate bool GameOverHandler(int finalScore, bool userWon);

  public interface IGameViewFramework
  {
    IGameSpriteFactory GetIGameSpriteFactory();
    Position GetScreenDimension();
    //callback for sending user input events to the controller
    //These could also be encapsulated in an class: SetInputHandler(UserInputHandler object) where object implements all callbacks.
    void SetMouseInputHandler(MouseInputHandler onMouseEvent);
    void SetKeyboardInputHandler(KeyboardInputHandler onKeyboardEvent);
    //blocking menu calls; these won't return until menus signal (via input, or simply animation-end as with level-completion menu)
    void OnLevelCompletion(int remainingCities, int remainingAmmo, int score);
    bool OnGameOver(int finalScore, bool userWon);
  }
}
