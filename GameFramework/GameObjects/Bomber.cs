/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.Kinematics;
using MissileCommand.EventSystem;
using MissileCommand.Interfaces;

namespace MissileCommand.GameObjects
{
  /*
   A bomber is armed with k payloads, which are missiles deployed at regular, spatial intervals
   (screen width div k). Bombers only move from left to right, for geometric simplicity.
   */
  public class Bomber : IGameObject
  {
    //the loaded-bomb array
    bool[] _bombs;
    // the screen width div the number of bombs to drop is (roughly) the bomb-drop interval
    int _dropInterval;
    //method to call when bomb should be dropped
    public BombDropEvent OnBombDrop;
    /// <summary>
    /// Method to call when the bomber is taken out by user.
    /// </summary>
    public DetonationHandler OnDetonation;

    int _id;
    public int Id { get { return _id; } set { _id = value; } }

    IGameSprite _sprite;
    public IGameSprite MySprite { get { return _sprite; } set { _sprite = value; } }

    Particle _particle;
    public Position Center { get { return _particle.position; } set { _particle.position = value; } }

    ObjectType _type;
    public ObjectType MyType { get { return _type; } set { _type = value; } }

    bool _persist = false;
    public bool PersistAfterDead { get { return _persist; } set { _persist = value; } }

    bool _stateChanged;
    public bool StateChanged { get { return _stateChanged; } set { _stateChanged = value; } }

    int _health;
    public int Health { get { return _health; } set { _health = value; } }

    double _hullRadius;
    public double HullRadius { get { return _hullRadius; } set { _hullRadius = value; } }

    bool _isTransparent = false;
    public bool IsTransparent { get { return _isTransparent; } set { _isTransparent = value; } }

    public Bomber(Particle startVector, IGameSprite gameSprite, int numBombs, int dropInterval)
    {
      _hullRadius = GameParameters.BOMBER_HULL_RADIUS;
      _type = ObjectType.BOMBER;
      _health = 100;
      _particle = startVector;
      _dropInterval = dropInterval;

      if (numBombs >= 0)
      {
        _bombs = new bool[numBombs];
        for (int i = 0; i < numBombs; i++)
        {
          _bombs[i] = true;
        }
      }
      else
      {
        Console.WriteLine("ERROR numBombs < 0 in Bomber ctor");
      }
      
      _sprite = gameSprite;      
    }

    //bomber is mostly passive to other objects; other objects act on it, not it upon them
    public void Interact(IGameObject other)
    {
      //nothing
    }

    /// <summary>
    /// Returns the distance from the center of other to the hull exterior of this object.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double HullDistance(IGameObject other)
    {
      return Position.Distance(_particle.position, other.Center) - _hullRadius - other.HullRadius;
    }

    void _move()
    {
      if (_health > 0)
      {
        Particle.Update(_particle);
      }
    }

    public void Update()
    {
      //call move before updating sprite
      _move();
      GameSpriteUpdateData updateData = new GameSpriteUpdateData(this.Center, 1.0);
      updateData.IsAlive = _health > 0;
      _sprite.Update(updateData);

      //Console.WriteLine("BOMBER health: {0}",_health);

      if (_health > 0)
      {
        //check for bombs to drop; this is equivalent to detecting a rising edge when each interval is reached
        int interval = (int)_particle.position.X / _dropInterval;
        if (interval < _bombs.Length)
        {
          if (_bombs[interval])
          {
            //interval reached and have not dropped bomb, so drop and mark as such
            _bombs[interval] = false;
            if (OnBombDrop != null)
            {
              //Console.WriteLine("BOMBER DROP");
              //this callback allows the listener to pick which bomb type to drop, so we can have different bomber behavior without altering this class
              OnBombDrop(_particle);
            }
          }
        }
      }
      //else dead, so call OnDetonation before this object is destroyed
      else 
      {
        if (OnDetonation != null)
        {
          Console.WriteLine(">>> Calling Bomber.OnDetonation");
          OnDetonation(_particle);
        }
        else
        { 
          Console.WriteLine("ERROR OnDetonate null in Bomber.Update()");
        }
      }
    }

    public bool IsFriend(IGameObject other)
    {
      return this.MyType != other.MyType && other.MyType != ObjectType.MISSILE && other.MyType != ObjectType.CURSOR;
    }
  }
}
