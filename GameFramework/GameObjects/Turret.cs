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
  public class Turret : IGameObject
  {
    int _ammunition;

    int _id;
    public int Id { get { return _id; } set { _id = value; } }

    IGameSprite _sprite;
    public IGameSprite MySprite { get { return _sprite; } set { _sprite = value; } }

    Position _center;
    public Position Center { get { return _center; } set { _center = value; } }

    ObjectType _type;
    public ObjectType MyType { get { return _type; } set { _type = value; } }

    bool _persist = false;
    public bool PersistAfterDead { get { return _persist; } set { _persist = value; } }

    bool _stateChanged;
    public bool StateChanged { get { return _stateChanged; } set { _stateChanged = value; } }

    int _health;
    public int Health
    {
      get { return _health; }
      set
      {
        _health = value;
        if (_health <= 0)
        {
          _sprite.Update(new GameSpriteUpdateData(_center, 0.0, false));
        }
      }
    }

    double _hullRadius;
    public double HullRadius { get { return _hullRadius; } set { _hullRadius = value; } }

    bool _isTransparent = false;
    public bool IsTransparent { get { return _isTransparent; } set { _isTransparent = value; } }

    public Turret(Position center, IGameSprite gameSprite)      
    {
      _center = center;
      _hullRadius = GameParameters.TURRET_HULL_RADIUS;
      _type = ObjectType.TURRET;
      _health = 100;
      _ammunition = GameParameters.TURRET_AMMO;
      _sprite = gameSprite;
    }

    public int GetAmmoCount()
    {
      return _ammunition;
    }

    public bool HasAmmo()
    {
      return _ammunition > 0;
    }

    public event TurretShotHandler OnShoot;

    //there may be different types of explosions, projectiles, etc: flak, ballistic, etc.
    public void Shoot(Position target)
    {
      if (_ammunition > 0 && _health > 0)
      {
        //spawn explosion???
        _ammunition--;
        if (OnShoot != null)
        {
          Position source = new Position(this.Center.X, this.Center.Y + this._sprite.GetHeight() / 2);
          OnShoot(source, target);
        }
        else
        {
          Console.WriteLine("ERROR OnShoot() event null in turret.shoot()");
        }
      }
      else
      {
        Console.WriteLine("ERROR turret out of ammo or dead, cannot shoot!");
      }
    }

    //not non-passive interactions for turret either, it just gets shot
    public void Interact(IGameObject other)
    {
      //nothing
    }

    public void Update()
    {
      GameSpriteUpdateData updateData = new GameSpriteUpdateData(this.Center, 1.0);
      updateData.IsAlive = _health > 0;
      updateData.Data = _ammunition.ToString();
      _sprite.Update(updateData);
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

    public bool IsFriend(IGameObject other)
    {
      return other.MyType != this.MyType && other.MyType != ObjectType.TURRET && other.MyType != ObjectType.TURRET_SHOT;
    }
  }
}
