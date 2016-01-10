/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.Kinematics;
using MissileCommand.Interfaces;

namespace MissileCommand.GameObjects
{
  public class City : IGameObject
  {
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

    //add delegates as needed here

    public City(Position center, IGameSprite gameSprite)
    {
      _hullRadius = GameParameters.CITY_HULL_RADIUS;
      _type = ObjectType.CITY;
      _health = GameParameters.CITY_HEALTH;
      _center = center;
      _sprite = gameSprite;
    }

    public void Interact(IGameObject other)
    {
      //city is passive, so do nothing

    }

    public bool IsFriend(IGameObject other)
    {
      return other.MyType != ObjectType.TURRET && other.MyType != this.MyType && other.MyType != ObjectType.TURRET_SHOT && other.MyType != ObjectType.CURSOR;
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
      GameSpriteUpdateData updateData = new GameSpriteUpdateData(this.Center, 1.0);
      updateData.IsAlive = _health > 0;
      _sprite.Update(updateData);
    }
  }
}
