using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MissileCommand.Kinematics;

namespace MissileCommand
{
  [Serializable]
  public class SpawnMirvEvent
  {
    //a particle descrbing the start position vector is the only payload
    public Particle StartVector;
    public double DeployAltitude;

    public SpawnMirvEvent(Particle startVector, double deployAltitude)
    {
      StartVector = new Particle(startVector);
      DeployAltitude = deployAltitude;
    }
  }
}
