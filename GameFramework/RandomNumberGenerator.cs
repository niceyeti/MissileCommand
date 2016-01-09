using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand
{
  public class RandomNumberGenerator
  {
    Random _random;
    static RandomNumberGenerator _instance = null;

    RandomNumberGenerator()
    {
      _random = new Random(DateTime.Now.Millisecond);
    }

    public static RandomNumberGenerator Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new RandomNumberGenerator();
        }
        return _instance;
      }
      private set{_instance = value;}
    }

    public int Rand()
    {
      return _random.Next();
    }
  }
}
