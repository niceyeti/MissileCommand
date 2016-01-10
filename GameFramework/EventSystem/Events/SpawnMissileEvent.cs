/* Copyright (c) 2015-2016 Jesse Waite */

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
  public class SpawnMissileEvent
  {
    //a particle descrbing the start position vector is the only payload
    public Particle _startVector { get; set; }

    public SpawnMissileEvent(Particle startVector)
    {
      _startVector = new Particle(startVector);
    }
  }
}
