using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand.Kinematics
{
  /// <summary>
  /// All kinematic and geometric junk
  /// </summary>
  [Serializable]
  public class Position
  {
    public Position(Position initialPosition)
    {
      X = initialPosition.X;
      Y = initialPosition.Y;
    }

    public Position(double x, double y)
    {
      X = x;
      Y = y;
    }

    public Position()
    {
      X = 0;
      Y = 0;
    }

    public double X { get; set; }
    public double Y { get; set; }


    /// <summary>
    /// Returns a random position near to the fixedPosition within some tolerance. This seems
    /// unusual, but is a very common desire for certain effects, like having a missile detonate
    /// and generate a bunch of random explosions within some general area.
    /// </summary>
    /// <returns>A new random position within a square of edge-length 2*modulus, centered at fixedPosition</returns>
    public static Position GetFuzzyPosition(Position fixedPosition, int modulus)
    {
      //generate random position, local to the fixed detonation position
      int x_fuzz = RandomNumberGenerator.Instance.Rand() % GameParameters.MISSILE_DETONATION_FUZZ_FACTOR;
      if (x_fuzz % 2 == 0)
      {
        x_fuzz *= -1;
      }

      int y_fuzz = RandomNumberGenerator.Instance.Rand() % GameParameters.MISSILE_DETONATION_FUZZ_FACTOR;
      if (y_fuzz % 2 == 0)
      {
        y_fuzz *= -1;
      }
      
      Position fuzzyPosition = new Position(fixedPosition);
      fuzzyPosition.X += x_fuzz;
      fuzzyPosition.Y += y_fuzz;

      return fuzzyPosition;
    }

    public static double Distance(Position pos1, Position pos2)
    {
      return System.Math.Sqrt(System.Math.Pow(pos1.X - pos2.X, 2.0) + System.Math.Pow(pos1.Y - pos2.Y, 2.0));
    }

    //TODO: these belong as static methods of the Position or Particle classes
    //returns angle of trajectory for particle at point 1 to reach point 2,
    //ranging from 0 - 2pi, of vector pointing from p1 to p2
    public static double pointTangent(Position p1, Position p2)
    {
      return pointTangent(p1.X, p1.Y, p2.X, p2.Y);
    }

    /// <summary>
    /// v1 values <x1,y1> are considered the pseduo-origin for measuring tan.
    /// v2 values <x2,y2> is the dest point.
    /// 
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns>Radian measurement to v2 relative to v1 as origin.</returns>
    public static double pointTangent(double x1, double y1, double x2, double y2)
    {
      double tangent = Math.Atan2(Math.Abs(y1 - y2), Math.Abs(x1 - x2));

      //map the quadrant
      // q1
      if (x1 <= x2 && y1 <= y2)
      {
        //do nothing, since tangent's value is already correct
        //tangent = tangent;
      }
      // q2
      else if (x1 >= x2 && y1 <= y2)
      {
        tangent = GameParameters.PI - tangent;
      }
      // q3
      else if (x1 >= x2 && y1 >= y2)
      {
        tangent = GameParameters.PI + tangent;
      }
      // q4
      else if (x1 <= x2 && y1 >= y2)
      {
        tangent = 2.0 * GameParameters.PI - tangent;
      }
      //unreachable
      else
      {
        Console.WriteLine("ERROR unmapped quadrant offset in pointTangent()");
      }

      //return tanQuadrantOffset plus tangent of vectors
      return tangent;
    }
  }
}
