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
  public class SpawnBomberEvent
  {
    //a particle descrbing the start position vector is the only payload
    public Position StartPosition;

    public SpawnBomberEvent(Position startPosition)
    {
      StartPosition = startPosition;
    }
  }
}
