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
  public class Missile : IGameObject
  {
    double _detonationProximity = GameParameters.DETONATION_PROXIMITY;

    int _id;
    public int Id { get { return _id; } set { _id = value; } }

    protected IGameSprite _sprite;
    public IGameSprite MySprite { get { return _sprite; } set { _sprite = value; } }

    public Position Center { get { return _particle.position; } set { _particle.position = value; } }

    protected ObjectType _type;
    public ObjectType MyType { get { return _type; } set { _type = value; } }

    bool _persist = false;
    public bool PersistAfterDead { get { return _persist; } set { _persist = value; } }

    bool _stateChanged;
    public bool StateChanged { get { return _stateChanged; } set { _stateChanged = value; } }

    protected int _health;
    public int Health
    {
      get { return _health; }
      set
      {
        _health = value;
        if (_health <= 0)
        {
          _sprite.Update(new GameSpriteUpdateData(_particle.position, 0.0, false));
        }
      }
    }

    double _hullRadius;
    public double HullRadius { get { return _hullRadius; } set { _hullRadius = value; } }

    bool _isTransparent = false;
    public bool IsTransparent { get { return _isTransparent; } set { _isTransparent = value; } }

    protected Particle _particle;

    /// <summary>
    /// These allow the missile to notify observers of detonation, allowing
    /// other objects to handle the detonation behavior.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public event DetonationHandler OnDetonation;

    public Missile(Particle newParticle, IGameSprite sprite)
    {
      _hullRadius = GameParameters.MISSILE_HULL_RADIUS;
      _type = ObjectType.MISSILE;
      _health = 10;
      _particle = newParticle;
      _sprite = sprite;
    }

    protected void _move()
    {
      Particle.Update(_particle);
      _sprite.Update(new GameSpriteUpdateData(this._particle.position, 1.0));
      GameSpriteUpdateData updateData = new GameSpriteUpdateData(this.Center, 1.0);
      updateData.IsAlive = _health > 0;
      _sprite.Update(updateData);
    }

    public virtual void Update()
    {
      //explode on ground impact
      if (_particle.position.Y <= GameParameters.GROUND_LEVEL)
      {
        _explode();
      }
      //update position
      else if (_health > 0)
      {
        _move();
      }
      else //health is <= 0, so kill sprite
      {
        _killSprite();
      }
    }

    public double HullDistance(IGameObject other)
    {
      return Position.Distance(this.Center, other.Center) - other.HullRadius - _hullRadius + 5;
    }

    //missile will harm bases and turrets
    public virtual bool IsFriend(IGameObject other)
    {
      return other.MyType == this.MyType || other.MyType == ObjectType.BOMBER || other.MyType == ObjectType.EXPLOSION || other.MyType == ObjectType.CURSOR;
    }

    protected void _explode()
    {
      _health = 0;
      //spawn explosion??? or let model do it
      //Console.WriteLine("EXPLOSION at " + _particle.position.X + ":" + _particle.position.Y);
      //fire callback to notify any observers of detonation
      if (OnDetonation != null)
      {
        OnDetonation(_particle);
      }

      _killSprite();
    }

    protected void _killSprite()
    {
      GameSpriteUpdateData updateData = new GameSpriteUpdateData(this.Center, 0.0);
      updateData.IsAlive = false;
      _sprite.Update(updateData);
    }

    //missiles will explode on any interaction involving a collision
    //missile will die and submit an explosion-event, which will be passed on to something that will spawn an explosion
    public void Interact(IGameObject other)
    {
      if (_health > 0)
      {
        if (!IsFriend(other))
        {
          if (other.MyType != ObjectType.TURRET_SHOT && other.MyType != ObjectType.AIR_BURST && HullDistance(other) <= 0)
          {
            _explode();
          }
        }
      }
    }
  }
}
