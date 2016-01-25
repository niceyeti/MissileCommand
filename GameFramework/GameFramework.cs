/* Copyright (c) 2015-2016 Jesse Waite */

using System.Threading;
using System;
using MissileCommand.EventSystem;
using MissileCommand.Levels;

namespace MissileCommand
{
  public class GameFramework
  {
    IGameViewFramework _viewFramework;
    Thread _frameworkThread;
    public Thread FrameworkThread
    {
      get { return _frameworkThread; }
    }

    bool IsRunning()
    {
      return _frameworkThread.ThreadState == ThreadState.Running;
    }

    public GameFramework(IGameViewFramework viewFramework)
    {
      _viewFramework = viewFramework;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
      //kick off the game framework thread, which runs all of the game logic independently of view
      _frameworkThread = new Thread(_run);
      _frameworkThread.Start();
    }

    /// <summary>
    /// TODO: Need a cleaner exit method for signaling models to clean up, possibly save game state, and exit.
    /// </summary>
    public void Exit()
    {
      _frameworkThread.Abort();
      if (_frameworkThread.IsAlive)
      {
        System.Console.WriteLine("ERROR GameFramework thread may not have exited properly...");
      }
      else
      {
        System.Console.WriteLine("GameFramework thread exited");
      }
    }

    /// <summary>
    /// 
    /// </summary>
    private void _run()
    {
      //initialize the game params first
      GameParameters.Initialize((int)_viewFramework.GetScreenDimension().X, (int)_viewFramework.GetScreenDimension().Y);
      
      GameLogger.Initialize(Console.Out);
      EventBus eventBus = new EventBus();
      EventFactory eventFactory = new EventFactory();
      EventMonitor eventMonitor = new EventMonitor(eventFactory,eventBus);
      //this thread will be blocked at GetIGameSpriteFactory() until ContentManager is initialized
      GameObjectFactory gameObjectFactory = new GameObjectFactory(eventMonitor, _viewFramework.GetIGameSpriteFactory());
      GameObjectContainer gameObjects = new GameObjectContainer();
      MainEventProcessor mainEventListener = new MainEventProcessor(gameObjects, gameObjectFactory, eventBus);
      GameModel gameModel = new GameModel(gameObjects, gameObjectFactory);
      GameController gameController = new GameController(eventBus, gameModel, eventFactory, _viewFramework);

      //controller runs forever-loop
      gameController.Run();
    }
  }
}
