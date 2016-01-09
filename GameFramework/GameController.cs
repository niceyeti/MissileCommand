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
    //IGameViewFramework gameView;
    EventFactory _eventFactory;

    public GameController(EventBus eventBus, GameModel model, EventFactory factory, IGameViewFramework viewFramework)
    {
      _gameModel = model;
      _eventBus = eventBus;
      _timerQ = new TimerQueue(eventBus);
      //gameView = iview;
      _eventFactory = factory;

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
      //Yes, time tick'ing is an internal property of the game framework, not actuated from outside. Remember
      //the desire is to have the game framework behave as a completely independent game implementation, whereas the
      //view is simply passive. So time must be defined here, within the game model, not as a public function (eg, 'Tick()').
      //Many kinematic properties of the game and other time-based parameters are also dependent on this time definition.
      System.Threading.Thread.Sleep((int)GameParameters.REFRESH_RATE_MS);
      //tick the timerQ. Do this before the eventSystem refresh, so any dequeued events are passed along for the refresh.
      _timerQ.Tick((int)GameParameters.REFRESH_RATE_MS);
      //pump the event queue
      _eventBus.ProcessAll();
    }
    
    //Loads up the timerQ with all pending game events (missiles, bombers, etc)
    //TODO: Move to Model?
    void _initializeTimedEventQ()
    {
      int spawnTimeout_ms;

      //init a few missiles as initial wave in the first few seconds, with no spawn timeout
      for (int i = 0; i < 5; i++)
      {
        spawnTimeout_ms = RandomNumberGenerator.Instance.Rand() % 1500;
        EventPacket missileEvent = _eventFactory.MakeRandomSpawnMissileEventPacket(_gameModel.GetGameObjects());
        TimedEvent timedMissile = new TimedEvent(spawnTimeout_ms, missileEvent);
        _timerQ.Insert(timedMissile);
      }
      //_timerQ.Print();
      //make 20 missiles, injected at random intervals, with a min separation of some sort
      for (int i = 0; i < 20; i++)
      {
        spawnTimeout_ms = RandomNumberGenerator.Instance.Rand() % GameParameters.LEVEL_DURATION_MS;
        EventPacket missileEvent = _eventFactory.MakeRandomSpawnMissileEventPacket(_gameModel.GetGameObjects());
        TimedEvent timedMissile = new TimedEvent(spawnTimeout_ms, missileEvent);
        _timerQ.Insert(timedMissile);
      }
      //_timerQ.Print();

      //the final wave at the end of the level
      for (int i = 0; i < 6; i++)
      {
        spawnTimeout_ms = GameParameters.LEVEL_DURATION_MS + RandomNumberGenerator.Instance.Rand() % 1500;
        EventPacket missileEvent = _eventFactory.MakeRandomSpawnMissileEventPacket(_gameModel.GetGameObjects());
        TimedEvent timedMissile = new TimedEvent(spawnTimeout_ms, missileEvent);
        _timerQ.Insert(timedMissile);
      }

      //some mirvs. what the hey.
      for (int i = 0; i < 6; i++)
      {
        spawnTimeout_ms = GameParameters.LEVEL_DURATION_MS + 1500 + RandomNumberGenerator.Instance.Rand() % 1500;
        EventPacket mirvEvent = _eventFactory.MakeRandomSpawnMirvEventPacket(_gameModel.GetGameObjects());
        TimedEvent timedMirv = new TimedEvent(spawnTimeout_ms, mirvEvent);
        _timerQ.Insert(timedMirv);
      }

      //spawn some bombers
      for (int i = 0; i < 3; i++)
      {
        spawnTimeout_ms = RandomNumberGenerator.Instance.Rand() % GameParameters.LEVEL_DURATION_MS;
        EventPacket bomberSpawn = _eventFactory.MakeSpawnBomberEventPacket(new Position(0,GameParameters.BOMBER_ALTITUDE));
        TimedEvent timedBomber = new TimedEvent(spawnTimeout_ms, bomberSpawn);
        _timerQ.Insert(timedBomber);
      }
    }

    /*
     Loads a level or other schema describing the initial state of a level.
     * TODO Input: a Level object, describing level state, difficulty, etc.
     */
    void initialize()
    {
      //init start up objects and state: bases, ammo, etc. Must be done before timerQ.
      _gameModel.Initialize();
      //must be done after model init
      _initializeTimedEventQ();
    }

    //run forever
    public void Run()
    {
      initialize();

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
