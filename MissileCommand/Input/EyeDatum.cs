/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Timers;

namespace MissileCommand.Input
{
  /// <summary>
  /// For algorithmic purposes, an eye-device input datum is defined by
  /// a MouseState (coordinates) and the timestamp of the datum. The timestamp
  /// could be absolute milliseconds as an int, but the .NET framework is more amenable
  /// to using DateTime instead.
  /// </summary>
  public class EyeDatum
  {
    public MouseState MouseData;
    public DateTime TimeStamp;

    public EyeDatum(MouseState mouseState, DateTime dateTime)
    {
      MouseData = mouseState;
      TimeStamp = dateTime;
    }

    public EyeDatum(EyeDatum datum)
    {
      MouseData = datum.MouseData;
      TimeStamp = datum.TimeStamp;
    }
  }
}
