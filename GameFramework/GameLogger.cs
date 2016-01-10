/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MissileCommand
{
  /*
   Global singleton class for logging game information.
   */
  public class GameLogger
  {
    static string _fileName = "gameLog.txt";
    static TextWriter _logFile;
    static GameLogger _instance = null;

    GameLogger(TextWriter writer)
    {
      _logFile = writer;
    }

    public static void Initialize(TextWriter writer)
    {
      _instance = new GameLogger(writer);
      _fileName = "streamredirected";
    }

    public static void Initialize(string fileName)
    {
      _fileName = fileName;
      _instance = new GameLogger(new StreamWriter(fileName));
    }

    public static GameLogger Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new GameLogger((TextWriter)Console.Out);
        }

        return _instance;
      }
    }

    public void Write(string text)
    {
      _logFile.Write(text);
    }

    public void Close()
    {
      if (_logFile != null)
      {
        _logFile.Close();
      }
    }

    public void Open(string fileName)
    {
      if (_logFile != null)
      {
        _logFile.Close();
      }

      _logFile = new System.IO.StreamWriter(fileName);
    }
  }
}
