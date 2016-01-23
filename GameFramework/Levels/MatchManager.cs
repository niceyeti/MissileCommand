using System;
using System.Collections.Generic;
using MissileCommand.TimedEventQueue;
using MissileCommand.Interfaces;
using MissileCommand.GameObjects;
using MissileCommand.EventSystem;
using MissileCommand.Kinematics;

namespace MissileCommand.Levels
{
  /// <summary>
  /// Match manager controls the execution of levels, the display of menus,
  /// and so on. The MatchManager thus overrides the controller's normal
  /// execution/tick'ing, to get user input, display game over, and to otherwise
  /// bound gameplay. It controls the initialization of levels, and their post-run
  /// logic, so primarily the data-object management. It does not do control logic over
  /// the levels; keep any of that in the controller.
  /// </summary>
  class MatchManager
  {
    string _user = "AAA";
    /// <summary>
    /// TODO: Convert levels into some class: GetNext(), IsNomore() ????
    /// </summary>
    List<Level> _levels;
    int _currentLevelIndex;
    Level _currentLevel;
    TimerQueue _timerQ;
    GameModel _gameModel;
    EventFactory _eventFactory;
    int _userScore;
    int _pointsPerCity;
    int _pointsPerRemainingShots;

    public LevelCompletionHandler OnLevelCompletion;
    public GameOverHandler OnGameOver;

    public MatchManager(GameModel gameModel, TimerQueue timedQ, EventFactory eventFactory, List<string> levelPaths)
    {
      _pointsPerCity = 100;
      _pointsPerRemainingShots = 10;
      _currentLevelIndex = 0;
      _userScore = 0;
      _timerQ = timedQ;
      _gameModel = gameModel;
      _levels = new List<Level>();
      _eventFactory = eventFactory;

      foreach (string path in levelPaths)
      {
        try
        {
          _levels.Add(new Level(path));
        }
        catch (Exception e)
        {
          Console.WriteLine("ERROR caught exception loading level from path: "+path+e.Message+e.StackTrace);
        }
      }

      //initiaize the first level
      _loadLevel(_levels[0]);
    }

    bool _enemiesExist(List<IGameObject> gameObjects)
    {
      bool enemiesExist = false;

      //searh for any remaining enemy objects
      foreach (IGameObject gameObject in gameObjects)
      {
        if (gameObject.MyType == ObjectType.BOMBER || gameObject.MyType == ObjectType.DEATH_HEAD || gameObject.MyType == ObjectType.EXPLOSION || gameObject.MyType == ObjectType.MISSILE)
        {
          enemiesExist = true;
        }
      }

      return enemiesExist;
    }

    /// <summary>
    /// Determines if level is complete by checking if there are any pending
    /// events, or if any enemy objects still exist.
    /// </summary>
    /// <param name="gameObjects"></param>
    /// <param name="timerQ"></param>
    /// <returns></returns>
    bool _isLevelComplete(List<IGameObject> gameObjects, TimerQueue timerQ)
    {
      bool isComplete = false;

      //level not complete if any timed events remain
      if (timerQ.Count() <= 0)
      {
        if (!_enemiesExist(gameObjects))
        {
          isComplete = true;
        }
      }

      return isComplete;
    }

    /// <summary>
    /// Get the user's score for this level, per old school MissileCommand scoring:
    /// 10 points per leftover missile, 100 points per remaining city.
    /// </summary>
    /// <param name="gameObjects"></param>
    /// <returns></returns>
    int _getLevelScore(List<IGameObject> gameObjects)
    {
      int score = 0;

      foreach (IGameObject gameObject in gameObjects)
      {
        if (gameObject.MyType == ObjectType.CITY)
        {
          score += _pointsPerCity;
        }
        if (gameObject.MyType == ObjectType.TURRET)
        {
          score += (((Turret)gameObject).GetAmmoCount() * _pointsPerRemainingShots);
        }
      }

      return score;
    }

    /// <summary>
    /// Returns game-over if no cities remain, or if no levels remain (which is a win).
    /// </summary>
    /// <param name="gameObjects"></param>
    /// <returns></returns>
    bool _isMatchOver(List<IGameObject> gameObjects)
    {
      bool citiesRemain, levelsRemain;

      citiesRemain = false;
      foreach (IGameObject gameObject in gameObjects)
      {
        if (gameObject.MyType == ObjectType.CITY && gameObject.Health > 0)
        {
          citiesRemain = true;
        }
      }

      levelsRemain = _levels.Count > 0;

      return !(levelsRemain || citiesRemain);
    }

    bool _isWin()
    {
      return true;
    }

    void _onLevelCompletion()
    {
      _userScore += _getLevelScore(_gameModel.GetGameObjects());

      if (OnLevelCompletion != null)
      {
        //Level completed, so notify view to display result, blocking until display has ended.
        OnLevelCompletion(_gameModel.GetGameObjects(), _userScore);
      }
      else
      {
        Console.WriteLine("ERROR OnLevelCompletion null in MatchManager");
      }
    }

    bool _updateMatch()
    {
      bool gameOver = false;

      //check if the user has reached game-over/game-end
      if (_isMatchOver(_gameModel.GetGameObjects()))
      {
        if (OnGameOver != null)
        {
          Console.WriteLine("GAME OVER Skipping game over menu...");
          //Notify view to display game-over animation and score, and get retry/quit selection from user.
          gameOver = !OnGameOver(_userScore, _isWin());
        }
        else
        {
          Console.WriteLine("ERROR OnLevelCompletion null in MatchManager");
        }
        _saveScore();
        _userScore = 0;
      }
      else
      {
        //else, update to the next level, or win!!
        _loadNextLevel();
      }

      return gameOver;
    }

    /// <summary>
    /// Updates the Match, returning the global exit status
    /// of the game. Updating the match entails checking for
    /// level completion, and subsequently checking if the game
    /// is over, displaying scores, and so on. In cases when gameplay
    /// ends with a menu, this function encapsulates a menu and returns
    /// the user's assent to continue/exit, like a winform.
    /// </summary>
    /// <returns></returns>
    public bool Update()
    {
      bool gameOver = false;

      //check the state of the current level
      if (!_isLevelComplete(_gameModel.GetGameObjects(), _timerQ))
      {
        //refresh the state of all objects in the model, only if level is in-progress
        _gameModel.Refresh();
      }
      else
      {
        //run level completion logic
        _onLevelCompletion();

        //update the match (check for game over, load new level, etc)
        gameOver = _updateMatch();
      }

      return gameOver;
    }

    /// <summary>
    /// GameModel.Initiaize() must be called before this, to make sure required objects exist.
    /// </summary>
    /// <param name="level"></param>
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

    void _loadLevel(Level level)
    {
      _currentLevel = level;
      _gameModel.Initialize();
      _initializeTimedEventQ(_currentLevel);
    }

    /// <summary>
    /// Load the next level in the list, in a wrapping fashion.
    /// </summary>
    /// <returns></returns>
    void _loadNextLevel()
    {
      _currentLevelIndex = (_currentLevelIndex + 1) % _levels.Count;
      _loadLevel(_levels[_currentLevelIndex]);
    }

    void _saveScore()
    {

    }
  }
}
