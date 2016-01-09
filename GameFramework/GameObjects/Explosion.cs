using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.Kinematics;
using MissileCommand.Interfaces;

namespace MissileCommand.GameObjects
{
  //many things create explosions, which dilate and then contract before disappearing, quickly
  public class Explosion : IGameObject
  {
    int _id;
    public int Id { get { return _id; } set { _id = value; } }

    IGameSprite _sprite;
    public IGameSprite MySprite { get { return _sprite; } set { _sprite = value; } }

    Position _center;
    public Position Center { get { return _center; } set { _center = value; } }

    protected ObjectType _type;
    public ObjectType MyType { get { return _type; } set { _type = value; } }

    bool _persist = false;
    public bool PersistAfterDead { get { return _persist; } set { _persist = value; } }

    bool _stateChanged;
    public bool StateChanged { get { return _stateChanged; } set { _stateChanged = value; } }

    int _health;
    public int Health 
    { 
      get { return _health; } 
      set { 
        _health = value;
        if (_health <= 0)
        {
          _sprite.Update(new GameSpriteUpdateData(_center, 0.0, false));
        }
      } 
    }

    protected double _hullRadius;
    public double HullRadius { get { return _hullRadius; } set { _hullRadius = value; } }

    bool _isTransparent = false;
    public bool IsTransparent { get { return _isTransparent; } set { _isTransparent = value; } }

    double _intensity;
    double _elapsed_ms;
    double _scalar;
    double _maxRadius = GameParameters.MAX_MISSILE_EXPLOSION_RADIUS;
    DateTime _start;
    double _duration_ms;
    protected double _damage;

    public Explosion(Position center, double intensity, IGameSprite gameSprite)
    {
      _scalar = 0;
      _start = DateTime.Now;
      _duration_ms = GameParameters.MISSILE_EXPLOSION_DURATION_MS;
      //explosion hull radius is a parabolic function of time
      _hullRadius = 0.0;
      _type = ObjectType.EXPLOSION;
      _health = 10;
      _intensity = intensity;
      _sprite = gameSprite;
      _center = center;
      _damage = GameParameters.EXPLOSION_DAMAGE;
    }

    /// <summary>
    /// A function defining the expansion/contraction of the explosion's radius
    /// as a scalar ranging from [0,1.0], as a function of time.
    /// </summary>
    /// <returns>a value ranging from 0-1.0 representing the sprite expansion/contraction scalar</returns>
    public double _updateScalar(double elapsed_ms)
    {
      double scalar = 0.0;

      //only update scalar if time remains
      if (elapsed_ms < _duration_ms)
      {
        scalar = Math.Sin(elapsed_ms * (Math.PI / _duration_ms));
      }

      return scalar;
    }

    /// <summary>
    /// Returns the distance from the center of other to the hull exterior of this object.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double HullDistance(IGameObject other)
    {
      return Position.Distance(_center, other.Center) - _hullRadius - other.HullRadius;
    }

    public void Update()
    {
      _elapsed_ms = (DateTime.Now - _start).TotalMilliseconds;

      if (_elapsed_ms >= _duration_ms)
      {
        //explosion expired, so let the model clean it up
        _scalar = 0.0;
        _hullRadius = 0.0;
        _health = 0;
      }
      else
      {
        _scalar = _updateScalar(_elapsed_ms);
        _hullRadius = _scalar * GameParameters.MAX_MISSILE_EXPLOSION_RADIUS;
      }

      _sprite.Update(new GameSpriteUpdateData(_center, _scalar, _health > 0));
    }

    //you dont interact with an explosion!
    public void Interact(IGameObject other)
    {
      if (_health > 0)
      {
        if (!IsFriend(other))
        {
          if (HullDistance(other) <= 0)
          {
            other.Health -= (int)_damage;
          }
        }
      }
    }

    public virtual bool IsFriend(IGameObject other)
    {
      return other.MyType == this.MyType || other.MyType == ObjectType.BOMBER || other.MyType == ObjectType.CURSOR || other.MyType == ObjectType.MISSILE;
    }
  }
}
