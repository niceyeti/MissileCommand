using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MissileCommand.Kinematics;

namespace MissileCommand.EventSystem
{
  [Serializable]
  public class SpawnTurretShotEvent
  {
    Position _source;
    Position _target;

    public Position Source { get { return _source; } set { _source = value; } }
    public Position Target { get { return _target; } set { _target = value; } }

    public SpawnTurretShotEvent(Position source, Position target)
    {
      _source = source;
      _target = target;
    }
  }
}
