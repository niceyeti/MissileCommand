/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand.Kinematics
{
  [Serializable]
  public class Particle
  {
    public Particle(Particle particle)
    {
      position = particle.position;
      velocity = particle.velocity;
      acceleration = particle.acceleration;
      theta = particle.theta;
    }

    public Particle(double vel, double acc, double ang, Position initialPosition)
    {
      position = initialPosition;
      velocity = vel;
      acceleration = acc;
      theta = ang;
      //Console.WriteLine("rad: " + ang);
    }

    //ignore the stupidity of these functions. they act on velocity/acceleration as scalars instead of vectors.
    //update position per velocity
    public static void UpdatePosition(Particle particle)
    {
      particle.position.X += (System.Math.Cos(particle.theta) * particle.velocity);
      particle.position.Y += (System.Math.Sin(particle.theta) * particle.velocity);
    }

    public static void Update(Particle particle)
    {
      Particle.UpdatePosition(particle);
      Particle.UpdateVelocity(particle);
    }

    public static void UpdateVelocity(Particle particle)
    {
      particle.velocity += particle.acceleration;
    }

    public Position position { get; set; }
    public double velocity { get; set; }
    public double acceleration { get; set; }
    public double theta { get; set; }
  }
}
