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
  class TurretShot : Missile
  {
    Position _target;

    public TurretShot(Particle newParticle, Position target, IGameSprite sprite) : base(newParticle,sprite)
    {
      _particle = newParticle;
      _particle.velocity = GameParameters.TURRET_SHOT_VELOCITY;
      _type = ObjectType.TURRET_SHOT;
      _target = target;
    }

    /// <summary>
    /// These allow the shot to notify observers of detonation, allowing
    /// other objects to handle the detonation behavior.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //public delegate void DetonationHandler(Object sender, Particle args);
    //public override event DetonationHandler OnDetonation;

    public bool HasExceededTarget()
    {
      bool hasExceeded = false;

      if (Center.Y >= GameParameters.GROUND_LEVEL)
      {
        //this gross simplification works because turret shots are always upward; minus 5 for tolerance
        hasExceeded = Center.Y >= _target.Y - 5;
        /* old, optimal, expensive solution
        //pointing into qaudrant I
        if (_particle.theta <= (GameParameters.PI / 2.0) && _particle.theta >= 0)
        {
          hasExceeded = Center._y > _target._y;
        }
        //pointing into quadrant II
        else if (_particle.theta >= (GameParameters.PI / 2.0) && _particle.theta < GameParameters.PI)
        {
          hasExceeded = Center._y > _target._y;
        }
        //pointing into quadrant III
        else if (_particle.theta >= GameParameters.PI && _particle.theta <= (3.0 * GameParameters.PI / 2.0))
        {
          hasExceeded = Center._y > _target._y;
        }
        //pointing into quadrant IV
        else if (_particle.theta >= (3.0 * GameParameters.PI / 2.0) && _particle.theta < (2.0 * GameParameters.PI))
        {
          hasExceeded = this.Center._y > _target._y;
        }
        else
        {
          Console.WriteLine("ERROR unreachable unmapped state in TurretShot.HasExceededTarget()");
        }
         */
      }

      return hasExceeded;
    }

    public override void Update()
    {
      //explode on ground impact
      if (HasExceededTarget())
      {
        _detonate();
      }
      //update position
      else if (Health > 0)
      {
        _move();
      }
      else //health is <= 0, so kill sprite
      {
        _killSprite();
      }
    }

    void _killSprite()
    {
      GameSpriteUpdateData updateData = new GameSpriteUpdateData(this.Center, 0.0);
      updateData.IsAlive = false;
      _sprite.Update(updateData);
    }

    public override bool IsFriend(IGameObject other)
    {
      return other.MyType == ObjectType.TURRET_SHOT || other.MyType == ObjectType.CITY || other.MyType == ObjectType.TURRET || other.MyType == ObjectType.CURSOR || other.MyType == ObjectType.AIR_BURST;
    }

    /*
    override void _move()
    {
      Particle.Update(_particle);
      _sprite.Update(new GameSpriteUpdateData(this._particle.position, 1.0));
      GameSpriteUpdateData updateData = new GameSpriteUpdateData(this.Center, 1.0);
      updateData.IsAlive = _health > 0;
      _sprite.Update(updateData);
    }
    */

    /// <summary>
    /// These allow the missile to notify observers of detonation, allowing
    /// other objects to handle the detonation behavior.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //public delegate void DetonationHandler(Object sender, Particle args);
    public event DetonationHandler OnProximityDetonation;

    void _detonate()
    {
      _health = 0;
      _killSprite();

      if (OnProximityDetonation != null)
      {
        OnProximityDetonation(_particle);
      }
      else
      {
        Console.WriteLine("ERROR OnProximityDetonation null in TurretShot._detonate()");
      }
    }
  }
}
