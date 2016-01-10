/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MissileCommand.Kinematics;

namespace MissileCommand.EventSystem.Events
{
  [Serializable]
  public class AirBurstEvent
  {
    Position _position;
    public Position Position { get { return _position; } set { _position = value; } }
    double _intensity;
    public double Intensity { get { return _intensity; } set { _intensity = value; } }

    public AirBurstEvent(Position center, double intensity)
    {
      _position = center;
      _intensity = intensity;
    }
  }
}
