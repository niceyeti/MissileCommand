using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.Kinematics;

namespace MissileCommand
{
  public interface IGameSpriteFactory
  {
    IGameSprite MakeMissileSprite(Position initialPosition);
    IGameSprite MakeMirvSprite(Position initialPosition);
    IGameSprite MakeTurretShotSprite(Position initialPosition);
    IGameSprite MakeExplosionSprite();
    IGameSprite MakeAirBurstSprite();
    IGameSprite MakeCitySprite();
    IGameSprite MakeTurretSprite();
    IGameSprite MakeBomberSprite();
  }
}
