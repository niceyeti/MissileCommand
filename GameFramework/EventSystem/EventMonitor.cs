using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.Kinematics;

namespace MissileCommand.EventSystem
{
  /// <summary>
  /// This class submits callbacks to game objects, and thereby listens for events.
  /// A primary example is submitting an OnDetonation() callback to Missile objects,
  /// such that external detonation logic can be implemented outside of the Missile class.
  /// Its best not to confuse this with other event-listeners. This class provides a means
  /// of inputting events to the event system, which are then processed by listeners. So
  /// this is really a mixture of behavioral and creational classes, removing event creation
  /// logic to outside of game objects. It listens for real events, not Event objects.
  /// 
  /// </summary>
  public class EventMonitor
  {
    EventFactory _eventFactory;
    EventBus _eventBus;

    public EventMonitor(EventFactory eventFactory, EventBus eventBus)
    {
      _eventFactory = eventFactory;
      _eventBus = eventBus;
    }

    public void OnAirBurstDetonation(Particle particle)
    {
      EventPacket packet = _eventFactory.MakeAirBurstEventPacket(particle.position);
      _eventBus.Receive(packet);    
    }

    /// <summary>
    /// Handler for when a bomber is taken out by friendly.
    /// </summary>
    /// <param name="particle"></param>
    public void OnBomberDetonation(Particle particle)
    {
      EventPacket packet = _eventFactory.MakeExplosionEventPacket(particle.position);
      _eventBus.Receive(packet);
    }

    /// <summary>
    /// Called when an object wishes to drop a bomb. The hostVector parameter is the kinematic
    /// description of the object dropping the bomb; this allows us to use this info however
    /// we want, even though not all of it will be used.
    /// </summary>
    /// <param name="hostVector"></param>
    public void OnBombDrop(Particle hostVector)
    {
      Particle startVector = new Particle(hostVector);
      startVector.velocity = GameParameters.MISSILE_VELOCITY;
      startVector.acceleration = GameParameters.MISSILE_ACCELERATION;
      //bomb theta is random, within some range constraint (a downward cone) centered at 3pi/2
      startVector.theta = (((double)RandomNumberGenerator.Instance.Rand()) / 47.0) % GameParameters.BOMB_THETA_FUZZ + 1.5 * GameParameters.PI - GameParameters.BOMB_THETA_FUZZ / 2.0;
      EventPacket packet = _eventFactory.MakeSpawnMissileEventPacket(startVector);
      _eventBus.Receive(packet);
    }

    /*
     When a mirv deploys, the missile separates into three separate mirvs.
     The primary continues on its trajectory; the other two
     branch off at plus/minus k degrees, respectively. The second mirvs are set
     * to deploy below 0, so they will effectively never deploy, just act as missiles.
     */
    public void OnMirvDeployment(Particle vector)
    {
      int velocityRandomizationPct = 15;

      //push the second mirv warhead into the event system
      Particle startParticle = new Particle(vector);
      //randomize the velocity a max of 15%
      int rand = RandomNumberGenerator.Instance.Rand();
      startParticle.velocity += ((double)(rand % velocityRandomizationPct) / 100.0);
      startParticle.theta = startParticle.theta + GameParameters.PI / 18;
      EventPacket mirv2 = _eventFactory.MakeSpawnMirvEventPacket(startParticle, GameParameters.MIN_Y);
      _eventBus.Receive(mirv2);

      //push the third mirv warhead
      startParticle = new Particle(vector);
      //randomize the velocity a max of 15%
      rand = RandomNumberGenerator.Instance.Rand();
      startParticle.velocity += ((double)(rand % velocityRandomizationPct) / 100.0);
      startParticle.theta = startParticle.theta - GameParameters.PI / 18;
      EventPacket mirv3 = _eventFactory.MakeSpawnMirvEventPacket(startParticle, GameParameters.MIN_Y);
      _eventBus.Receive(mirv3);
    }

    public void OnMissileDetonation(Particle particle)
    {
      EventPacket packet = _eventFactory.MakeExplosionEventPacket(particle.position);
      _eventBus.Receive(packet);
    }

    public void OnTurretShot(Position source, Position target)
    {
      EventPacket packet = _eventFactory.MakeTurretShotEventPacket(source, target);
      _eventBus.Receive(packet);
    }
  }
}
