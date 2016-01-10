/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Concurrent;

namespace MissileCommand.Input
{
  /// <summary>
  /// This is the class in which to experiment with different eye input acquisition, filtering,
  /// and interpretive methods. This class could be subject to a lot of decomposition; it implements
  /// data aquisition, filtering, and inference models all in one class. However, these overlap in many
  /// ways, and enforcing a particular software pattern on them just entails a lot of rewrite. In other
  /// words, cohesion is low because the nature of the problem lacks cohesion.
  /// 
  /// This EyeDevice implementation is asynchronous, since eye input may or may not be ready at
  /// the time GetMouseState() is called by some client, and more importantly, this object must
  /// continually poll for new mouse data.
  /// </summary>
  public class EyeDevice : IMouseDevice
  {
    //TODO: faster data structure? ring?
    int _misses;
    Object _newDatumLock;
    RingBuffer<EyeDatum> _eyeDataBuffer;
    EyeDatum _newestDatum;
    EyeSensorFilter _filter;
    bool _pendingDatum;
    //polls the sensor
    Thread _pollingThread;
    //the rate at which the polling thread will sample the mouse sensor
    int _sensorScanTime_ms;
    //computes the state of the mouse, then sleeps until woken by new data
    Thread _computationThread;
    bool _exit;
    MouseState _currentState;
    int _computationScanTime_ms;
    int SENSOR_DATA_HISTORY;

    public EyeDevice(int sensorHz)
    {
      _exit = false;
      _pendingDatum = false;
      //the data history should exceed the Hz of sensors on the market; this gives 2 seconds history for a 250Hz sensor.
      //A larger value could be used for historical or other vector analysis on the extended inputs.
      SENSOR_DATA_HISTORY = 500;
      _eyeDataBuffer = new RingBuffer<EyeDatum>(SENSOR_DATA_HISTORY);
      _filter = new EyeSensorFilter();
      _newDatumLock = new Object();

      //the number of pending datum misses by the consumer thread
      _misses = 0;

      if (sensorHz >= 500)
      {
        Console.WriteLine("ERROR sensorHz="+sensorHz+" is to high sensitivity for sensor poll. Large (>500Hz) sensorHz values");
        Console.WriteLine("may prevent sensor data from being read. Re-evaluate algorithms in EyeDevice.");
      }

      //_currentState must have some initial value, or readers will get null ref
      _currentState = Mouse.GetState();

      //Set the poll scan time. Note this math is intentionally incorrect: using 1200 instead of 1000 is intended so that the
      //sensor-polling thread always runs slightly faster than the rate at which manufacturers claim for their tracker Hz.
      _sensorScanTime_ms = 1200 / sensorHz;
      _computationScanTime_ms = _sensorScanTime_ms;
      //launch the sensor polling thread.
      _pollingThread = new Thread(new ThreadStart(_pollSensor));
      _pollingThread.Start();

      //kick the computation thread
      _computationThread = new Thread(new ThreadStart(_runComputations));
      _computationThread.Start();
    }

    /// <summary>
    /// This shall always return a value, which shall always be the most recently calculated
    /// mouse state (the output of all filtering and approximation algorithms).
    /// </summary>
    /// <returns></returns>
    public MouseState GetMouseState()
    {
      return _currentState;
    }

    //Probably unnecessary.
    void Die()
    {
      if (_pollingThread != null && _pollingThread.IsAlive)
      {
        _pollingThread.Abort();
      }

      if (_computationThread != null && _computationThread.IsAlive)
      {
        _computationThread.Abort();
      }

      _exit = true;
    }

    /// <summary>
    /// The sensor polling thread main method. The polling thread wakes periodically
    /// to check for new input, waking at approximately the refresh rate of the sensor
    /// (usually between 50 and several hundred Hz). Likely, its better to have the
    /// thread wake slightly more often than the sensor Hz rate, due to hardware slippage
    /// or other inaccuracies in reported Hz.
    /// </summary>
    void _pollSensor()
    {
      while (!_exit)
      {
        //Consider sensor scanning as O(1), so don't include read op as an expense upon scan time;
        //the below also requires locking to be very fast, behavior that must be promised by other lock'ers.
        _setNewDatum();

        //sleep at approximately the same rate as the sensor Hz
        System.Threading.Thread.Sleep(_sensorScanTime_ms);
      }
    }

    /// <summary>
    /// Multi-threaded symmetic counterpart of GetNewDatum. Set _newestDatum to the
    /// current MouseState. Old datum is simply overwritten. The motivation for this design
    /// is so that the sensor-data history data structure does not need to be blocking.
    /// The consumer thread runs computations as fast as it can upon the most recent data, but
    /// neither thread should block because of pending data.
    /// 
    /// TODO: evaluate if only changed data should be added; if mouse state has not changed within
    /// a very brief time interval, the mouse state likely has not changed.
    /// </summary>
    void _setNewDatum()
    {
      lock (_newDatumLock)
      {
        if (_pendingDatum)
        {
          //increment misses; this is just for debugging how poorly the consumer is consuming new data.
          _misses++;
        }

        _newestDatum = new EyeDatum(Mouse.GetState(), DateTime.Now);
        _pendingDatum = true;
      }
    }

    /// <summary>
    /// Multithreaded counterpart of SetNewDatum(), this gets the newest datum.
    /// </summary>
    EyeDatum _getNewDatum()
    {
      EyeDatum datum = null;

      if (_pendingDatum)
      {
        //acquire new data and push it
        lock (_newDatumLock)
        {
          //copy the newest datum
          datum = new EyeDatum(_newestDatum);
          _pendingDatum = false;
        }
      }

      return datum;
    }

    /// <summary>
    /// The computation thread; gets the most recent datum, updates/cleans the EyeDatum queue, and most
    /// importantly, runs computations over recent input.
    /// </summary>
    void _runComputations()
    {
      int elapsed_ms, remaining_ms;
      DateTime startTime;
      Vector2 coordinates;
      bool isLeftButtonClicked;

      while (!_exit)
      {
        //record the start time
        startTime = DateTime.Now;

        //get the latest datum, if any, and add it to the sensor data
        EyeDatum newDatum = _getNewDatum();
        if (newDatum != null)
        {
          //push new data to ring
          _eyeDataBuffer.Push(newDatum);
        }

        //run filtering algorithms to set current estimated mouse state coordinates
        coordinates = _filter.GetEstimatedCoordinates(_eyeDataBuffer);
        
        //run click inference
        isLeftButtonClicked = _filter.DetectLeftClick(_eyeDataBuffer);

        //update the public MouseState
        _updateMouseState(coordinates,isLeftButtonClicked);

        //get elapsed computation time
        elapsed_ms = (int)(DateTime.Now - startTime).TotalMilliseconds;

        //sleep for remaining time left in this scan
        remaining_ms = _computationScanTime_ms - elapsed_ms;
        if (remaining_ms > 0)
        {
          System.Threading.Thread.Sleep(remaining_ms);
        }
        else 
        {
          Console.WriteLine("WARNING no time left in computation scan interval. Elapsed milliseconds ="+elapsed_ms);
        }
      }
    }

    void _updateMouseState(Vector2 coordinates, bool isLeftButtonClicked)
    {
      //set the current estimated state using a new MouseState
      ButtonState leftButtonState = isLeftButtonClicked ? ButtonState.Pressed : ButtonState.Released;
      _currentState = new MouseState((int)coordinates.X, (int)coordinates.Y, 0, leftButtonState, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
    }
  }
}
