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
  /*An air borne form of explosion, created by a turret shot. This is a friendly
   * explosion, protecting bases and  destroying up enemy items. The only difference between
   * this and enemy missile explosions is the IsFriend() method, and potentially the size
   * and nature of the explosion.
   * */
  public class AirBurst : Explosion
  {
    public AirBurst(Position center, double intensity, IGameSprite gameSprite): base(center, intensity, gameSprite)
    {
      _type = ObjectType.AIR_BURST;
      _damage = GameParameters.AIR_BURST_DAMAGE;
      _hullRadius = 0.0;
    }

    public override bool IsFriend(IGameObject other)
    {
      return other.MyType == ObjectType.AIR_BURST || other.MyType == ObjectType.CITY || other.MyType == ObjectType.CURSOR || other.MyType == ObjectType.TURRET || other.MyType == ObjectType.TURRET_SHOT;
    }
  }
}
