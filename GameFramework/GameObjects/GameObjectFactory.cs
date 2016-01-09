using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.GameObjects;
using MissileCommand.Kinematics;
using MissileCommand.EventSystem;
using MissileCommand.Interfaces;

namespace MissileCommand
{
  /*
   Should be able to shove all creational, extrinsic stuff in here, so client code doesn't have to contain any of that,
   and can only contain the runtime logic.
   * 
   * Passing in the SpriteFactory is smart because each game object only references its most memory expensive components,
   * allowing us to flyweight out the images, textures, and all that for a particular game object across all similar objects.
   */
  public class GameObjectFactory
  {
    EventMonitor _eventMonitor;
    IGameSpriteFactory _spriteFactory;

    public GameObjectFactory(EventMonitor eventMonitor, IGameSpriteFactory gameSpriteFactory)
    {
      _eventMonitor = eventMonitor;
      _spriteFactory = gameSpriteFactory;
    }

    public IGameObject MakeAirBurst(Position center, double intensity)
    {
      IGameSprite airBurstSprite = _spriteFactory.MakeAirBurstSprite();
      AirBurst airBurst = new AirBurst(center, intensity, airBurstSprite);

      return (IGameObject)airBurst;
    }

    public IGameObject MakeMirv(Particle initialVector, double deploymentAltitude)
    {
      IGameSprite mirvSprite = _spriteFactory.MakeMirvSprite(initialVector.position);
      Mirv mirv = new Mirv(initialVector, mirvSprite, deploymentAltitude);
      mirv.OnMirvDeployment += _eventMonitor.OnMirvDeployment;
      mirv.OnDetonation += _eventMonitor.OnMissileDetonation;

      return (IGameObject)mirv;
    }

    public IGameObject MakeExplosion(Position center, double intensity)
    {
      IGameSprite explosionSprite = _spriteFactory.MakeExplosionSprite();
      Explosion explosion = new Explosion(center, intensity, explosionSprite);

      return (IGameObject)explosion;
    }

    public IGameObject MakeMissile(Particle initialVector)
    {
      IGameSprite missileSprite = _spriteFactory.MakeMissileSprite(initialVector.position);
      Missile missile = new Missile(initialVector, missileSprite);
      missile.OnDetonation += _eventMonitor.OnMissileDetonation;
      
      return (IGameObject)missile;
    }

    public IGameObject MakeTurretShot(Particle initialVector, Position target)
    {
      IGameSprite turretShotSprite = _spriteFactory.MakeTurretShotSprite(initialVector.position);
      TurretShot turretShot = new TurretShot(initialVector, target, turretShotSprite);
      turretShot.OnProximityDetonation += _eventMonitor.OnAirBurstDetonation;

      return (IGameObject)turretShot;
    }

    public IGameObject MakeTurret(Position center)
    {
      IGameSprite turretSprite = _spriteFactory.MakeTurretSprite();
      Turret turret = new Turret(center,turretSprite);
      turret.OnShoot += _eventMonitor.OnTurretShot;

      return (IGameObject)turret;
    }

    public IGameObject MakeCity(Position center)
    {
      IGameSprite citySprite = _spriteFactory.MakeCitySprite();
      City city = new City(center,citySprite);

      return (IGameObject)city;
    }

    /*
     * TODO: MOve this up a level, pass particle into fact
    Bomber start vectors aren't very random, they start at relatively the same altitude,
    and all proceed from left to right across the screen at slow speed.
     */
    Particle _getRandomBomberStartVector()
    {
      //get the random height, within some band of altitude
      int randY = RandomNumberGenerator.Instance.Rand() % GameParameters.BOMBER_ALTITUDE_FUZZ + GameParameters.BOMBER_ALTITUDE;
      Position startPosition = new Position(0.0,(double)randY);
      //bomber proceeds direct from left to right, so 0 rad.
      double theta = 0.0;
      double velocity = RandomNumberGenerator.Instance.Rand() % GameParameters.BOMBER_VELOCITY_FUZZ + GameParameters.BOMBER_VELOCITY;
      return new Particle(velocity, 0.0, theta, startPosition);
    }

    public IGameObject MakeBomber(Position center)
    {
      IGameSprite bomberSprite = _spriteFactory.MakeBomberSprite();
      Particle startVector = _getRandomBomberStartVector();
      int dropInterval = GameParameters.MAX_X / GameParameters.BOMBER_AMMO + RandomNumberGenerator.Instance.Rand() % GameParameters.BOMBER_AMMO;
      Bomber bomber =  new Bomber(startVector, bomberSprite, GameParameters.BOMBER_AMMO, dropInterval);
      bomber.OnBombDrop += _eventMonitor.OnBombDrop;
      bomber.OnDetonation += _eventMonitor.OnBomberDetonation;
      return bomber;
    }
  }
}
