using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.GameObjects;
using MissileCommand.Kinematics;
using MissileCommand.EventSystem.Events;
using Microsoft.Xna.Framework.Input;
using MissileCommand.Interfaces;

namespace MissileCommand.EventSystem
{
  /*
   * Constructs events in a manner sufficient to submit them to the EventSystem.
   * 
   * Can gameObjects be gotten rid of??? alternatively, pass it in
   * 
   */
  public class EventFactory
  {
    public EventFactory()
    {
    }

    public EventPacket MakeExplosionEventPacket(Position center)
    {
      return MakeExplosionEventPacket(center, GameParameters.MISSILE_EXPLOSION_INTENSITY);
    }

    public EventPacket MakeAirBurstEventPacket(Position center)
    {
      return MakeAirBurstEventPacket(center, GameParameters.AIR_BURST_INTENSITY);
    }

    public EventPacket MakeMouseEventPacket(MouseState mouseState, Position mousePosition)
    {
      MouseEvent mouseEvent = new MouseEvent(mouseState,mousePosition);
      byte[] data = EventCodec.Instance.Serialize<MouseEvent>(mouseEvent);
      EventPacket packet = new EventPacket(EventTypes.MOUSE_INPUT, data);
      return packet;
    }

    public EventPacket MakeKeyboardEventPacket(KeyboardState keyboardState)
    {
      KeyboardEvent keyboardEvent = new KeyboardEvent(keyboardState);
      byte[] data = EventCodec.Instance.Serialize<KeyboardEvent>(keyboardEvent);
      EventPacket packet = new EventPacket(EventTypes.KEYBOARD_INPUT, data);
      return packet;
    }

    public EventPacket MakeExplosionEventPacket(Position position, double intensity)
    {
      ExplosionEvent explosion = new ExplosionEvent(position, intensity);
      Console.WriteLine("EvtFact, position is: " + position.X + ":" + position.Y);
      byte[] data = EventCodec.Instance.Serialize<ExplosionEvent>(explosion);
      EventPacket packet = new EventPacket(EventTypes.EXPLOSION, data);
      return packet;
    }

    public EventPacket MakeAirBurstEventPacket(Position position, double intensity)
    {
      AirBurstEvent airBurstEvent = new AirBurstEvent(position, intensity);
      Console.WriteLine("EvtFact, airburst position is: " + position.X + ":" + position.Y);
      byte[] data = EventCodec.Instance.Serialize<AirBurstEvent>(airBurstEvent);
      EventPacket packet = new EventPacket(EventTypes.AIR_BURST, data);
      return packet;
    }

    /// <summary>
    /// Given an initialization vector, returns a packet describing a missile spawn event.
    /// </summary>
    /// <param name="startVector"></param>
    /// <returns>A missile spawn event packet.</returns>
    public EventPacket MakeSpawnMissileEventPacket(Particle startVector)
    {
      SpawnMissileEvent missileSpawn = new SpawnMissileEvent(startVector);
      byte[] eventData = EventCodec.Instance.Serialize<SpawnMissileEvent>(missileSpawn);
      EventPacket eventPacket = new EventPacket(EventTypes.SPAWN_MISSILE, eventData);

      return eventPacket;
    }

    public EventPacket MakeSpawnMirvEventPacket(Particle startVector, double deployAltitude)
    {
      SpawnMirvEvent missileSpawn = new SpawnMirvEvent(startVector,deployAltitude);
      byte[] eventData = EventCodec.Instance.Serialize<SpawnMirvEvent>(missileSpawn);
      EventPacket eventPacket = new EventPacket(EventTypes.SPAWN_MIRV, eventData);

      return eventPacket;
    }

    public EventPacket MakeTurretShotEventPacket(Position source, Position target)
    {
      SpawnTurretShotEvent turretShot = new SpawnTurretShotEvent(source, target);
      byte[] eventData = EventCodec.Instance.Serialize<SpawnTurretShotEvent>(turretShot);
      EventPacket eventPacket = new EventPacket(EventTypes.TURRET_SHOT, eventData);

      return eventPacket;
    }

    /// <summary>
    /// For now, this parameterless version just assumes build a random start position
    /// for the bomber. There isn't much randomness except the height.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <returns></returns>
    public EventPacket MakeSpawnBomberEventPacket(Position startPosition)
    {
      SpawnBomberEvent bomberEvent = new SpawnBomberEvent(startPosition);
      byte[] data = EventCodec.Instance.Serialize<SpawnBomberEvent>(bomberEvent);
      EventPacket eventPacket = new EventPacket(EventTypes.SPAWN_BOMBER, data);

      return eventPacket;
    }

    /// <summary>
    /// Selects a random target base as target, and builds an event packet around it. This smells
    /// like business logic that belongs outside of this class... alternatively we could pass in the
    /// game-object list dependency for selecting a random target.
    /// </summary>
    /// <returns></returns>
    public EventPacket MakeRandomSpawnMirvEventPacket(List<IGameObject> gameObjects)
    {
      //initialize a random start location along the top border
      int xCoor = RandomNumberGenerator.Instance.Rand() % GameParameters.MAX_X;
      Position startPosition = new Position(xCoor, GameParameters.MAX_Y);
      //all enemy missiles point at a random city or base, in radians
      double angleRad = _getTargetTrajectory(startPosition, gameObjects);
      Particle startVector = new Particle(GameParameters.MISSILE_VELOCITY, GameParameters.MISSILE_ACCELERATION, angleRad, startPosition);
      EventPacket packet = MakeSpawnMirvEventPacket(startVector,GameParameters.MIRV_DEPLOY_ALTITUDE);

      return packet;
    }

    /// <summary>
    /// Selects a random target base as target, and builds an event packet around it. This smells
    /// like business logic that belongs outside of this class... alternatively we could pass in the
    /// game-object list dependency for selecting a random target.
    /// </summary>
    /// <returns></returns>
    public EventPacket MakeRandomSpawnMissileEventPacket(List<IGameObject> gameObjects)
    {
      //initialize a random start location along the top border
      int xCoor = RandomNumberGenerator.Instance.Rand() % GameParameters.MAX_X;
      Position startPosition = new Position(xCoor, GameParameters.MAX_Y);
      //all enemy missiles point at a random city or base, in radians
      double angleRad = _getTargetTrajectory(startPosition, gameObjects);
      Particle startVector = new Particle(GameParameters.MISSILE_VELOCITY, GameParameters.MISSILE_ACCELERATION, angleRad, startPosition);
      EventPacket packet = MakeSpawnMissileEventPacket(startVector);

      return packet;
    }

    //of the bases, select a random one and return its reference
    IGameObject _selectRandomTarget(List<IGameObject> gameObjects)
    {
      int i = 0;
      IGameObject target = null;
      int randomBaseIndex = RandomNumberGenerator.Instance.Rand() % GameParameters.NUM_CITIES;

      foreach (IGameObject gameObject in gameObjects)
      {
        if (gameObject.MyType == ObjectType.CITY || gameObject.MyType == ObjectType.TURRET)
        {
          if (i == randomBaseIndex)
          {
            target = gameObject;
            break;
          }
          i++;
        }
      }

      return target;
    }

    //Returns direction of a city or base w.r.t. some start location
    //TODO: where does this belong? model? city class?
    double _getTargetTrajectory(Position missileStartPosition, List<IGameObject> gameObjects)
    {
      IGameObject targetBase = _selectRandomTarget(gameObjects);
      double rad = 0;

      if (targetBase != null)
      {
        Console.WriteLine("missile pos: " + missileStartPosition.X + ":" + missileStartPosition.Y);
        Console.WriteLine("base pos: " + targetBase.Center.X + ":" + targetBase.Center.Y);
        rad = Position.pointTangent(missileStartPosition, targetBase.Center);
      }
      else
      {
        GameLogger.Instance.Write("ERROR targetBase null in getTargetTrajectory()");
      }

      return rad;
    }
  }
}
