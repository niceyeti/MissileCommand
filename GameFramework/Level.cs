/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MissileCommand
{
  /// <summary>
  /// An object defining a level. This just parses and then exposes level data.
  /// </summary>
  public class Level
  {
    const string _bomberXpath = "/level/bombers";
    const string _deathheadXpath = "/level/deathheads";
    const string _missileXpath = "/level/missiles";
    const string _mirvXpath = "/level/mirvs";

    int _numMissiles;
    int _numMirvs;
    int _numDeathheads;
    int _numBombers;

    public int NumMissiles
    {
      get { return _numMissiles; }
      private set { _numMissiles = value; }
    }

    public int NumMirvs
    {
      get { return _numMirvs; }
      private set { _numMirvs = value; }
    }

    public int NumDeathheads
    {
      get { return _numDeathheads; }
      private set { _numDeathheads = value; }
    }

    public int NumBombers
    {
      get { return _numBombers; }
      private set { _numBombers = value; }
    }

    public Level()
    {
    
    }

    public Level(string levelPath)
    {
      Load(levelPath);
    }

    int _stringToInt(string str)
    {
      int n = 0;

      try
      {
        if (!int.TryParse(str, out n))
        {
          Console.WriteLine("ERROR could not parse string to int: " + str);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("ERROR could not parse string to int: " + str + e.Message + e.StackTrace);
      }

      return n;
    }

    /// <summary>
    /// Loads a file from the xml file given by path.
    /// </summary>
    /// <param name="path"></param>
    public void Load(string path)
    {
      XmlDocument xdoc = new XmlDocument();

      try
      {
        xdoc.Load(path);
        string missiles = xdoc.SelectSingleNode(_missileXpath).InnerText;
        string mirvs = xdoc.SelectSingleNode(_mirvXpath).InnerText;
        string bombers = xdoc.SelectSingleNode(_bomberXpath).InnerText;
        string deathheads = xdoc.SelectSingleNode(_deathheadXpath).InnerText;

        _numMissiles = _stringToInt(missiles);
        _numMirvs = _stringToInt(mirvs);
        _numBombers = _stringToInt(bombers);
        _numDeathheads = _stringToInt(deathheads);
      }
      catch (Exception e)
      {
        Console.WriteLine("ERROR exception caught loading level: "+e.Message+e.StackTrace);
      }
    }
  }
}
