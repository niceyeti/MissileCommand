/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MissileCommand.EventSystem;
using MissileCommand.TimedEventQueue;
using Microsoft.Xna.Framework.Input;
using MissileCommand.Kinematics;
using MissileCommand.Interfaces;

namespace MissileCommand
{
  /*
   This should implement most of the game characteristics (level, ticks, user input events, enemy spawning, etc).
   Model just handles the relational stuff.
   
   * This is a God class for now, but could just be broken up later as patterns become apparent.
   */
  class GameController
  {
    bool _exit = false;
    GameModel _gameModel;
    EventBus _eventBus;
    TimerQueue _timerQ;
    EventFactory _eventFactory;
    string _level1Path = @"\Level1.xml";
    string _levelRootFolder = Environment.CurrentDirectory+"\\Levels";

    public GameController(EventBus eventBus, GameModel model, EventFactory factory, IGameViewFramework viewFramework)
    {
      _gameModel = model;
      _eventBus = eventBus;
      _timerQ = new TimerQueue(eventBus);
      _eventFactory = factory;
      _level1Path = _levelRootFolder + _level1Path;

      //TODO: setting these could be moved outside this constructor. This object needs no persistent reference to view.
      viewFramework.SetKeyboardInputHandler(OnKeyboardInput);
      viewFramework.SetMouseInputHandler(OnMouseInput);
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
      System.Threading.Thread.Sleep((int)GameParameters.REFRESH_RATE_MS);
      //tick the timerQ. Do this before the eventSystem refresh, so any dequeued events are passed along for the refresh.
      _timerQ.Tick((int)GameParameters.REFRESH_RATE_MS);
      //pump the event queue
      _eventBus.ProcessAll();
    }

    void _initializeTimedEventQ(Level level)
    {
      int i, spawnTimeout_ms;

      //start by emptying the queue; it should never be full in this context
      _timerQ.Clear();

      //init five missiles as initial wave in the first few seconds, with no spawn timeout
      for (i = 0; i < 5; i++)
      {
        spawnTimeout_ms = RandomNumberGenerator.Instance.Rand() % 1500;
        EventPacket missileEvent = _eventFactory.MakeRandomSpawnMissileEventPacket(_gameModel.GetGameObjects());
        TimedEvent timedMissile = new TimedEvent(spawnTimeout_ms, missileEvent);
        _timerQ.Insert(timedMissile);
      }

      //make 20 missiles, injected at random intervals, with a min separation of some sort
      for (i = 0; i < level.NumMissiles; i++)
      {
        spawnTimeout_ms = RandomNumberGenerator.Instance.Rand() % GameParameters.LEVEL_DURATION_MS;
        EventPacket missileEvent = _eventFactory.MakeRandomSpawnMissileEventPacket(_gameModel.GetGameObjects());
        TimedEvent timedMissile = new TimedEvent(spawnTimeout_ms, missileEvent);
        _timerQ.Insert(timedMissile);
      }

      //the final wave of missiles at the end of the level
      for (i = 0; i < 5; i++)
      {
        spawnTimeout_ms = GameParameters.LEVEL_DURATION_MS + RandomNumberGenerator.Instance.Rand() % 1500;
        EventPacket missileEvent = _eventFactory.MakeRandomSpawnMissileEventPacket(_gameModel.GetGameObjects());
        TimedEvent timedMissile = new TimedEvent(spawnTimeout_ms, missileEvent);
        _timerQ.Insert(timedMissile);
      }

      //some mirvs at the end
      for (i = 0; i < level.NumMirvs; i++)
      {
        spawnTimeout_ms = GameParameters.LEVEL_DURATION_MS + 1500 + RandomNumberGenerator.Instance.Rand() % 1500;
        EventPacket mirvEvent = _eventFactory.MakeRandomSpawnMirvEventPacket(_gameModel.GetGameObjects());
        TimedEvent timedMirv = new TimedEvent(spawnTimeout_ms, mirvEvent);
        _timerQ.Insert(timedMirv);
      }

      //spawn the bombers
      for (i = 0; i < level.NumBombers; i++)
      {
        spawnTimeout_ms = RandomNumberGenerator.Instance.Rand() % GameParameters.LEVEL_DURATION_MS;
        EventPacket bomberSpawn = _eventFactory.MakeSpawnBomberEventPacket(new Position(0, GameParameters.BOMBER_ALTITUDE));
        TimedEvent timedBomber = new TimedEvent(spawnTimeout_ms, bomberSpawn);
        _timerQ.Insert(timedBomber);
      }

      //spawn the deathheads
      for (i = 0; i < level.NumDeathheads; i++)
      {
        spawnTimeout_ms = RandomNumberGenerator.Instance.Rand() % GameParameters.LEVEL_DURATION_MS;
        EventPacket deathheadSpawn = _eventFactory.MakeSpawnBomberEventPacket(new Position(0, GameParameters.BOMBER_ALTITUDE));
        TimedEvent timedDeathhead = new TimedEvent(spawnTimeout_ms, deathheadSpawn);
        _timerQ.Insert(timedDeathhead);
      }
    }

    /// <summary>
    /// Loads a level by constructing a level object and using it to re-initialize
    /// the TimedEventQ, the primary driver of the game.
    /// </summary>
    /// <param name="levelPath"></param>
    void _loadLevel(string levelPath)
    {
      Level _level = new Level(levelPath);
      _initializeTimedEventQ(_level);
    }

    /*
     Loads a level or other schema describing the initial state of a level.
     * TODO Input: a Level object, describing level state, difficulty, etc.
     */
    void _initialize()
    {
      //init start up objects and state: bases, ammo, etc. Must be done before timerQ.
      _gameModel.Initialize();
      //must be done after model init
      //_initializeTimedEventQ();
      _loadLevel(_level1Path);
    }

    //run forever
    public void Run()
    {
      _initialize();

      while (!_exit)
      {
        _tick();
        _gameModel.Refresh();
      }

      _clean();
    }

    //TODO? 
    void _clean()
    {}

    public void Exit()
    {
      _exit = true;
    }
  }
}
