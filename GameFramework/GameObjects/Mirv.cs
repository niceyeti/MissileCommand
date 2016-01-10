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
  /*
   A mirv is just a missile which separates into multiple warheads at some specified altitude.
   This is done by detecting the altitude, and then spawning SpawnMissileEvents with + and - some
   fixed angle (say, 15 degrees, or pi/12 rad).
   */
  class Mirv : Missile
  {
    double _deployAltitude;
    bool _once;

    public delegate void MirvDeploymentHandler(Particle args);
    public event MirvDeploymentHandler OnMirvDeployment;

    public Mirv(Particle newParticle, IGameSprite sprite, double deployAltitude) : base(newParticle,sprite)
    {
      _deployAltitude = deployAltitude;
      _once = true;
      _type = ObjectType.MISSILE;
    }

    /*
     The only difference between mirv and missile is the mirv spawns a bunch of other missiles at slightly
     * different angles, after reaching a certain altitude.
     */
    public override void Update()
    {
      base.Update();

      if (_once && _particle.position.Y <= _deployAltitude && _health > 0)
      {
        _once = false;

        //deployment altitude reached, so deploy mirvs
        if (OnMirvDeployment != null)
        {
          OnMirvDeployment(_particle);
        }
      }
    }
  }
}
