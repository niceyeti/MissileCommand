/* Copyright (c) 2015-2016 Jesse Waite */

using System;

namespace MissileCommand
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
  public static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      bool eyeMode = false;
      //detect the eye device parameter
      foreach (string arg in Environment.GetCommandLineArgs())
      {
        if (arg.ToLower() == "--eye")
        {
          Console.WriteLine("HAS EYE");
          eyeMode = true;
        }
      }

      GameViewFramework viewFramework = new GameViewFramework(eyeMode);
      GameFramework missileCommand = new GameFramework((IGameViewFramework)viewFramework);
      //start the game framework, which runs all of the game logic
      missileCommand.Start();

      //start the view process, which just passively receives updates from the framework
      try
      {
        viewFramework.Run();
      }
      catch (Exception e)
      {
        System.Console.WriteLine("Main loop caught exception: "+e.Message+"\n\n"+e.StackTrace);
      }
      System.Console.WriteLine("Game view framework exited");
      missileCommand.Exit();
    }
  }
#endif
}
