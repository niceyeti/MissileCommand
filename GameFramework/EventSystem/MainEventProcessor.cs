/* Copyright (c) 2015-2016 Jesse Waite */

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
   Implements the observer side of the EventSystem. For a large game, there would be many
   EventWatchers (game subsystems). For now, just one global watcher should suffice, eg,
   simple causal logic: if missile detonation, spawn explosion. if turret fired, queue
   artillery shell, etc.
   
   This class sniffs most (likely all, for now) events, mapping events to outcomes.
   
   Hence this class subscribes to all events, granularity which clearly isn't proper,
   but works fine for such a small game as MissileCommand.
   */
  class MainEventProcessor : IObserver
  {
    GameObjectFactory _objectFactory;
    GameObjectContainer _objectContainer;
    EventBus _eventBus;

    public MainEventProcessor(GameObjectContainer gameObjectContainer, GameObjectFactory factory, EventBus eventBus)
    {
      _objectFactory = factory;
      _objectContainer = gameObjectContainer;
      _eventBus = eventBus;
      _initializeSubscriptions();
    }

    void _initializeSubscriptions()
    {
      //subscribe to all events this proc should listen for
      List<EventTypes> events = new List<EventTypes>();
      events.Add(EventTypes.CITY_DESTROYED);
      events.Add(EventTypes.COLLISION);
      events.Add(EventTypes.EXPLOSION);
      events.Add(EventTypes.OUT_OF_AMMO);
      events.Add(EventTypes.SPAWN_MISSILE);
      events.Add(EventTypes.SPAWN_MIRV);
      events.Add(EventTypes.SPAWN_BOMBER);
      events.Add(EventTypes.TURRET_SHOT);
      events.Add(EventTypes.MOUSE_INPUT);
      events.Add(EventTypes.KEYBOARD_INPUT);
      events.Add(EventTypes.AIR_BURST);
      _eventBus.SubscribeMultiple(this, events);
    }

    void spawnExplosion(EventPacket newEvent)
    {
      //Position position = EventCodec.Instance.BytesToPosition(newEvent.Data);
      ExplosionEvent explosionEvent = EventCodec.Instance.Deserialize<ExplosionEvent>(newEvent.Data);
      IGameObject explosion = _objectFactory.MakeExplosion(explosionEvent.Position, explosionEvent.Intensity);
      _objectContainer.Add(explosion);
    }

    /// <summary>
    /// spawnExplosion() spawns an explosion at an exact location. This version spawns an
    /// explosion at a random location very near to the position specific by event, for a nice
    /// multi-explosion effect.
    /// </summary>
    /// <param name="newEvent"></param>
    void spawnFuzzyExplosion(EventPacket newEvent)
    {
      //Position position = EventCodec.Instance.BytesToPosition(newEvent.Data);
      ExplosionEvent explosionEvent = EventCodec.Instance.Deserialize<ExplosionEvent>(newEvent.Data);
      Position fuzzyPosition = Position.GetFuzzyPosition(explosionEvent.Position, GameParameters.MISSILE_DETONATION_FUZZ_FACTOR);
      IGameObject explosion = _objectFactory.MakeExplosion(fuzzyPosition, explosionEvent.Intensity);
      _objectContainer.Add(explosion);
    }

    void spawnAirBurst(EventPacket newEvent)
    {
      //Position position = EventCodec.Instance.BytesToPosition(newEvent.Data);
      AirBurstEvent explosionEvent = EventCodec.Instance.Deserialize<AirBurstEvent>(newEvent.Data);
      IGameObject airburst = _objectFactory.MakeAirBurst(explosionEvent.Position, explosionEvent.Intensity);
      _objectContainer.Add(airburst);
    }

    /// <summary>
    /// spawnExplosion() spawns an explosion at an exact location. This version spawns an
    /// explosion at a random location very near to the position specific by event, for a nice
    /// multi-explosion effect.
    /// </summary>
    /// <param name="newEvent"></param>
    void spawnFuzzyAirBurst(EventPacket newEvent)
    {
      AirBurstEvent airBurstEvent = EventCodec.Instance.Deserialize<AirBurstEvent>(newEvent.Data);
      Position fuzzyPosition = Position.GetFuzzyPosition(airBurstEvent.Position, GameParameters.MISSILE_DETONATION_FUZZ_FACTOR);

      IGameObject explosion = _objectFactory.MakeAirBurst(fuzzyPosition, airBurstEvent.Intensity);
      _objectContainer.Add(explosion);
    }
    
    void spawnMissile(EventPacket newEvent)
    {
      SpawnMissileEvent missileSpawn = EventCodec.Instance.Deserialize<SpawnMissileEvent>(newEvent.Data);
      IGameObject missile = _objectFactory.MakeMissile(missileSpawn._startVector);
      _objectContainer.Add(missile);
    }

    void spawnBomber(EventPacket newEvent)
    {
      SpawnBomberEvent bomberEvent = EventCodec.Instance.Deserialize<SpawnBomberEvent>(newEvent.Data);
      IGameObject bomber = _objectFactory.MakeBomber(bomberEvent.StartPosition);
      _objectContainer.Add(bomber);
    }

    void spawnMirv(EventPacket newEvent)
    {
      SpawnMirvEvent mirvEvent = EventCodec.Instance.Deserialize<SpawnMirvEvent>(newEvent.Data);
      IGameObject mirv = _objectFactory.MakeMirv(mirvEvent.StartVector, mirvEvent.DeployAltitude);
      _objectContainer.Add(mirv);
    }

    void spawnTurretShot(EventPacket newEvent)
    {
      SpawnTurretShotEvent turretShotEvent = EventCodec.Instance.Deserialize<SpawnTurretShotEvent>(newEvent.Data);
      double ang = Position.pointTangent(turretShotEvent.Source,turretShotEvent.Target);
      Particle initialVector = new Particle(GameParameters.TURRET_SHOT_VELOCITY, GameParameters.TURRET_SHOT_ACCELERATION, ang, turretShotEvent.Source);
      IGameObject turretShot = _objectFactory.MakeTurretShot(initialVector,turretShotEvent.Target);
      _objectContainer.Add(turretShot);
    }

    /// <summary>
    /// Selects a random turret to take the shot.
    /// </summary>
    /// <param name="target"></param>
    void fireTurret(Position target)
    {
      List<IGameObject> armedTurrets = _objectContainer.ToList().Where(obj => (obj.MyType == ObjectType.TURRET && obj.Health > 0 && ((Turret)obj).HasAmmo())).ToList<IGameObject>();

      if (armedTurrets.Count > 0)
      {
        int randomTurret = RandomNumberGenerator.Instance.Rand() % armedTurrets.Count;
        ((Turret)armedTurrets[randomTurret]).Shoot(target);
      }
      else
      {
        Console.WriteLine("NO ARMED TURRETS");
      }
    }

    /// <summary>
    /// Only discrete mouse clicks are handled for now, which trigger
    /// turret shots, if in active region.
    /// </summary>
    void handleMouseClick(EventPacket newEvent)
    {
      MouseEvent mouseEvent = EventCodec.Instance.Deserialize<MouseEvent>(newEvent.Data);

      //only detect left click rising edge
      if (mouseEvent.IsLeftClick)
      {
        Position target = mouseEvent.EventPosition;
        fireTurret(target);
      }
    }

    /*
     Could demux these using Chain of Responsibility pattern: have this class deque events,
     * then iterate list of listeners, cal dispatch on each, first that returns true break loop
     * and pass event into object.
     */
    public void Notify(EventPacket newEvent)
    {
      switch (newEvent.Type)
      {
        case EventTypes.AIR_BURST:
          spawnAirBurst(newEvent);
          spawnFuzzyAirBurst(newEvent);
          spawnFuzzyAirBurst(newEvent);
          break;
        case EventTypes.SPAWN_MIRV:
          spawnMirv(newEvent);
          break;
        case EventTypes.CITY_DESTROYED:
          break;
        case EventTypes.SPAWN_BOMBER:
          spawnBomber(newEvent);
          break;
        case EventTypes.SPAWN_MISSILE:
          spawnMissile(newEvent);
          break;
        case EventTypes.COLLISION:
          break;
        case EventTypes.EXPLOSION:
          spawnExplosion(newEvent);
          spawnFuzzyExplosion(newEvent);
          spawnFuzzyExplosion(newEvent);
          break;
        case EventTypes.OUT_OF_AMMO:
          break;
        case EventTypes.TURRET_SHOT:
          spawnTurretShot(newEvent);
          break;
        case EventTypes.MOUSE_INPUT:
          handleMouseClick(newEvent);
          break;
        case EventTypes.KEYBOARD_INPUT:
          //Console.WriteLine("Keyboard input received as null...");
          break;
        default:
          Console.WriteLine("ERROR ExplosionObserver notified of unmapped event type: " + newEvent.Type.ToString());
          //GameLogger.Instance.Write("ERROR ExplosionObserver notified of unmapped event type: " + newEvent.Type.ToString());
          break;
      }
    }
  }
}
