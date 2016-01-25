/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using MissileCommand.EventSystem;
using MissileCommand.TimedEventQueue;
using Microsoft.Xna.Framework.Input;
using MissileCommand.Kinematics;
using MissileCommand.Levels;

namespace MissileCommand
{
  /*
   This should implement most of the game characteristics (level, ticks, user input events, enemy spawning, etc).
   Model just handles the relational stuff.
   */
  class GameController
  {
    bool _gameOver = false;
    GameModel _gameModel;
    EventBus _eventBus;
    TimerQueue _timerQ;
    EventFactory _eventFactory;
    List<string> _levels;
    MatchManager _matchManager;
    int _tickMS;

    public GameController(EventBus eventBus, GameModel model, EventFactory factory, IGameViewFramework viewFramework)
    {
      _gameModel = model;
      _eventBus = eventBus;
      _timerQ = new TimerQueue(eventBus);
      _eventFactory = factory;
      _tickMS = (int)GameParameters.REFRESH_RATE_MS;

      //TODO: The level paths should be an input from some game configuration file, instead of hardcoding them here
      //initialize the level paths. Does this belon here, or in model?
      _levels = new List<string>();
      for (int i = 1; i <= 3; i++)
      {
        _levels.Add(Environment.CurrentDirectory + "\\Levels\\Level" + i.ToString() + ".xml");
      }
      _matchManager = new MatchManager(model, _timerQ, eventBus, _eventFactory, _levels);

      //TODO: setting these could be moved outside this constructor. This object needs no persistent reference to view.
      viewFramework.SetKeyboardInputHandler(OnKeyboardInput);
      viewFramework.SetMouseInputHandler(OnMouseInput);
      _matchManager.OnLevelCompletion += viewFramework.OnLevelCompletion;
      _matchManager.OnGameOver += viewFramework.OnGameOver;
    }

    /// <summary>
    /// The all-encompassin millisecond refresh tick rate of the game model.
    /// This is the time input to all kinematics within the game.
    /// This must be well-factored, so the game execution can be automated
    /// in a high speed and viewless fashion; it is also useful for speeding
    /// up gameplay, such as when the user has no more missiles and the game
    /// is sped up so they don't have to wait long for the level to conclude.
    /// 
    /// </summary>
    public int TickMS
    {
      get
      {
        return _tickMS;
      }
      set
      {
        _tickMS = value;
      }
    }


    /// <summary>
    /// This is the callback called by the view when the user inputs a key.
    /// Enqueues key events to the event system. Let listeners handle the output logic
    /// of key events, just receive and forward them here.
    /// </summary>
    /// <param name="keyboardState"></param>
    public void OnKeyboardInput(KeyboardState keyboardState)
    {
      EventPacket eventPacket = _eventFactory.MakeKeyboardEventPacket(keyboardState);
      _eventBus.Receive(eventPacket);
    }

    /// <summary>
    /// Callback called when user inputs a mouse event. The event is just packaged and
    /// forward to the event system.
    /// 
    /// TODO: This makes the eventBus concurrent!!! Synchronize writers.
    /// </summary>
    /// <param name="mouseState"></param>
    public void OnMouseInput(MouseState mouseState, Position mousePosition)
    {
      EventPacket eventPacket = _eventFactory.MakeMouseEventPacket(mouseState,mousePosition);
      _eventBus.Receive(eventPacket);    
    }

    /// <summary>
    /// 
    /// </summary>
    void _tick()
    {
      //Yes, time tick'ing is an internal property of the game framework, not actuated from outside.
      //The desire is to have the game framework behave as a completely independent game implementation, whereas the
      //view is simply passive. So time must be defined here, within the game model, not as a public function (eg, 'Tick()').
      //Many kinematic properties of the game and other time-based parameters are also dependent on this time definition.
      System.Threading.Thread.Sleep(_tickMS);
      //tick the timerQ. Do this before the eventSystem refresh, so any dequeued events are passed along for the refresh.
      _timerQ.Tick((int)GameParameters.REFRESH_RATE_MS);
      //pump the event queue
      _eventBus.ProcessAll();
    }

    /// <summary>
    /// Run forever, until no more levels, or user quits. Not clear if this belongs in Controller;
    /// </summary>
    public void Run()
    {
      _gameOver = false;

      while (!_gameOver)
      {
        //update the match
        _gameOver = _matchManager.Update();
        //actuate time
        _tick();
      }

      _clean();
    }

    //TODO? 
    void _clean()
    {}

    public void Exit()
    {
      _gameOver = true;
    }
  }
}
