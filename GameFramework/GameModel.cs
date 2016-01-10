/* Copyright (c) 2015-2016 Jesse Waite */

using System.Threading;
using System.Linq;
using MissileCommand.GameObjects;
using MissileCommand.Kinematics;
using System;
using System.Collections.Generic;
using MissileCommand.EventSystem;
using MissileCommand.TimedEventQueue;
using MissileCommand.Interfaces;

namespace MissileCommand
{
  /*
   GameModel likely needs to be broken into GameModel and Application class.
   GameModel should not both trigger the event processing, and also be a subscriber to
   events. Just seems weird. Or maybe GameModel is fine, but we need aIGameObject manager
   to encapsulate the interaction, creation, and destruction of objects. Make
   GameModel just the outer container for GameSubsystems.
   * 
   */
  //The passive model for the game (update rules, score, levels, etc)
  public class GameModel
  {
    public GameModel(GameObjectContainer objects, EventBus eventSys, GameObjectFactory objectFactory)
    {
      _minSeparation = GameParameters.MIN_SEPARATION_HEURISTIC_VALUE;
      _gameObjectFactory = objectFactory;
      _gameObjectCollection = objects;
    }

    double _minSeparation; //double the minimum bounding radius of the largest object
   GameObjectContainer _gameObjectCollection;
   GameObjectFactory _gameObjectFactory;

    /// <summary>
    /// Returns game objects as a list of IGameObjects. This should mostly be for readonly access.
    /// </summary>
    /// 
    /// <returns></returns>
   public List<IGameObject> GetGameObjects()
   {
     return _gameObjectCollection.ToList();
   }

    /// <summary>
    /// 
    /// </summary>
    public void Refresh()
    {
        //update the state of all objects
        _update();
        //clean up dead objects, etc.
        _clean();
    }

    //TODO: pass in an initial-state object of some kind (xml, etc)
    public void Initialize()
    {
      //this offset is used to init a layout like original MissileCommand:  turret - city city city - turret - city city city - turret
      float offset = (float)(GameParameters.MAX_X - GameParameters.GROUND_SPRITE_SPACING) / (float)9;
      int groundSpriteHorizontal = (int)(GameParameters.MIN_Y + GameParameters.GROUND_SPRITE_HEIGHT / 2);

      //init three bases, three turrets, each with 10 rounds of ammo
      IGameObject turret1 = _gameObjectFactory.MakeTurret(new Position((int)offset * 0 + GameParameters.GROUND_SPRITE_SPACING, groundSpriteHorizontal));
      IGameObject turret2 = _gameObjectFactory.MakeTurret(new Position((int)offset * 4 + GameParameters.GROUND_SPRITE_SPACING, groundSpriteHorizontal));
      IGameObject turret3 = _gameObjectFactory.MakeTurret(new Position((int)offset * 8 + GameParameters.GROUND_SPRITE_SPACING, groundSpriteHorizontal));
      _gameObjectCollection.Add(turret1);
      _gameObjectCollection.Add(turret2);
      _gameObjectCollection.Add(turret3);

      //init the cities
      IGameObject city1 = _gameObjectFactory.MakeCity(new Position((int)(offset * 1 + GameParameters.GROUND_SPRITE_SPACING), groundSpriteHorizontal));
      IGameObject city2 = _gameObjectFactory.MakeCity(new Position((int)(offset * 2 + GameParameters.GROUND_SPRITE_SPACING), groundSpriteHorizontal));
      IGameObject city3 = _gameObjectFactory.MakeCity(new Position((int)(offset * 3 + GameParameters.GROUND_SPRITE_SPACING), groundSpriteHorizontal));
      IGameObject city4 = _gameObjectFactory.MakeCity(new Position((int)(offset * 5 + GameParameters.GROUND_SPRITE_SPACING), groundSpriteHorizontal));
      IGameObject city5 = _gameObjectFactory.MakeCity(new Position((int)(offset * 6 + GameParameters.GROUND_SPRITE_SPACING), groundSpriteHorizontal));
      IGameObject city6 = _gameObjectFactory.MakeCity(new Position((int)(offset * 7 + GameParameters.GROUND_SPRITE_SPACING), groundSpriteHorizontal));
      _gameObjectCollection.Add(city1);
      _gameObjectCollection.Add(city2);
      _gameObjectCollection.Add(city3);
      _gameObjectCollection.Add(city4);
      _gameObjectCollection.Add(city5);
      _gameObjectCollection.Add(city6);
    }
    
    //housekeeping on dead objects and the like
    void _clean()
    {
      //Console.WriteLine("before clean |objs|=" + IGameObjectCollection.Size());
      //removes all expired objects
      //TODO: clean this. It creates an entire new list and substitutes it with the old one
      _gameObjectCollection.SetObjects(_gameObjectCollection.ToList().Where(obj => obj.Health > 0).ToList());
      //TODO garbage collect
      //Console.WriteLine("after clean |objs|=" + IGameObjectCollection.Size());
    }

    //detects and returns if two objects may share some interaction, given their distance or other attributes.
    //allows us to avert comparing objects that can't share any interaction
    bool _inRange(IGameObject obj1,IGameObject obj2)
    {
      if (Math.Abs(obj1.Center.X - obj2.Center.X) < GameParameters.MIN_SEPARATION_HEURISTIC_VALUE)
      {
        if (Math.Abs(obj1.Center.Y - obj2.Center.Y) < GameParameters.MIN_SEPARATION_HEURISTIC_VALUE)
        {
          return true;
        }
      }

      return false;
    }

    //simple O(n^2) update of all objects A and B in model
    void _update()
    {
      int i, j;

      //These indices may look fishy, but are correct. Both object Interact() methods are called in the innermost
      //loop, but the indices guarantee an object's 
      for (i = 0; i < _gameObjectCollection.Size() - 1; i++)
      {
        for (j = i + 1; j < _gameObjectCollection.Size(); j++)
        {
          IGameObject obj_i = _gameObjectCollection.Get(i);
          IGameObject obj_j = _gameObjectCollection.Get(j);
          //optimization: only compare different object types
          if (obj_i.MyType != obj_j.MyType)
          {
            //small optimization: only compare objects within some min spatial range. Note this could be problematic in terms of assumed interaction.
            if (_inRange(obj_i, obj_j))
            {
              //update first object's state
              obj_i.Interact(_gameObjectCollection.Get(j));
              //update second object's state
              obj_j.Interact(_gameObjectCollection.Get(i));
            }
          }
        }
      }

      //run the static update of each object, after running the bidirectional/inter-object updates/interactions
      foreach (IGameObject obj in _gameObjectCollection)
      {
        obj.Update();
        /*
        if (obj.MyType == ObjectType.EXPLOSION)
        {
          Console.WriteLine("Explosion location: "+obj.Center._x + ":"+obj.Center._y);
        }
        */
      }
    }
  }
}