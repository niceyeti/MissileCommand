using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.Kinematics;

namespace MissileCommand
{
  /*
   This is a data transfer class that provides a nice data description of
   * the data transfer between game-objects in the model and their view-sprites.
   */
  public class GameSpriteUpdateData
  {
    public Position NewPosition;
    public double SizeScalar;
    public bool IsAlive;
    public string Data;

    public GameSpriteUpdateData(Position newPosition, double sizeScalar)
    {
      NewPosition = newPosition;
      SizeScalar = sizeScalar;
      IsAlive = true;
      //user of this object is responsible for creating and serializing data[]
      Data = null;
    }

    public GameSpriteUpdateData(Position newPosition, double sizeScalar, bool isAlive)
    {
      NewPosition = newPosition;
      SizeScalar = sizeScalar;
      IsAlive = isAlive;
    }
  }
}
