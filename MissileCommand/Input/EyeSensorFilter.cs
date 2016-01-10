/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Timers;

namespace MissileCommand.Input
{
  /// <summary>
  /// The class for receiving raw eye device inputs (typically just by reading the mouse state).
  /// Don't be confused by the name; though this class can do filtering (input removal and/or interpretation),
  /// it is a tool for performing filtering. Clients may use its methods to do so; it should not encapsulate or
  /// otherwise hide filtering concepts in its implementation.
  /// </summary>
  public class EyeSensorFilter
  {
    //RingBuffer<EyeDatum> _sensorData;

    public EyeSensorFilter()
    {
      //_sensorData = new RingBuffer<EyeDatum>();
    }

    /// <summary>
    /// Returns the product of the stdev of the x and y components of the previous n mouse states,
    /// which makes for a very good input trigger. There are many optimizations available for this function.
    /// 
    /// The stdev strategy parameters must be based upon the physical problem of detecting clicks via a
    /// concentration of points within a region. This means that the Hz rate of the sensor is an input,
    /// as well as other physical characteristics of the device and its current calibration.
    /// 
    /// There need be no weighting in this function either; since we assume the user is focused on a given point, all
    /// points captured within a sample of time window t are estimates of the true 'hidden' value of the current points.
    /// </summary>
    /// <param name="numPoints">The number of previous points to include in the stdev-xy product.</param>
    /// <returns></returns>
    public double StDevXyProduct(int numPoints, RingBuffer<EyeDatum> sensorData)
    {
      int i;
      double mu_x = 0.0, mu_y = 0.0;
      double var_x = 0.0, var_y = 0.0;

      //get the means
      for (i = 0; i < numPoints && i < sensorData.Count; i++)
      {
        mu_x += sensorData[i].MouseData.X;
        mu_y += sensorData[i].MouseData.Y;
      }
      mu_x /= (double)i;
      mu_y /= (double)i;
      
      //get the variance of the x and y coordinates in this set
      for (i = 0; i < numPoints && i < sensorData.Count; i++)
      {
        var_x += ((sensorData[i].MouseData.X - mu_x) * (sensorData[i].MouseData.X - mu_x));
        var_y += ((sensorData[i].MouseData.Y - mu_y) * (sensorData[i].MouseData.Y - mu_y));
      }

      //optimization TODO: there is no need for expensive square-root for the trigger we're looking for;
      //by laws of exponents, (x^0.5)(y^0.5) == (x*y)^0.5, and thus the sqrt is actually unnecessary for
      //a trigger, since the trigger baseline can simply be squared. And also if we're careful wrt overflow.
      return Math.Sqrt(var_x) * Math.Sqrt(var_y);
    }

    /// <summary>
    /// Estimates the current cursor location by a weighted average of the previous k points.
    /// This function assumes eyeData is a list with most recent values at front.
    /// </summary>
    /// <param name="eyeData"></param>
    /// <returns></returns>
    public Vector2 GetEstimatedCoordinates(RingBuffer<EyeDatum> eyeData)
    {
      if (eyeData.Count == 0)
      {
        //TODO: this is an error condition
        return new Vector2(0, 0);
      }

      //call the coordinate-average method, about the simplest method
      Vector2 estimate =  _coordinateAverageMethod(3, eyeData);
      // regression/predictive or dy/dx method
      // ...

      return estimate;
    }

    Vector2 _coordinateAverageMethod(int cacheWindow, RingBuffer<EyeDatum> eyeData)
    {
      int i;
      float mu_x = 0, mu_y = 0;
      //float denom = 0, weight;

      //tapered/weighted average of x and y coordinates over last k inputs
      for (i = 0; i < cacheWindow && i < eyeData.Count; i++)
      {
        //non-weighted summation
        mu_x += (float)eyeData[i].MouseData.X;
        mu_y += (float)eyeData[i].MouseData.Y;
        //weighted summation
        //weight = (float)i + 1;
        //mu_x += ((float)eyeData[i].MouseData.X / weight);
        //mu_y += ((float)eyeData[i].MouseData.Y / weight);
        //accumulate the denominator, since it isn't just i
        //denom += weight;
      }

      //return new Vector2(mu_x / denom, mu_y / denom);
      return new Vector2(mu_x / (float)i, mu_y / (float)i);
    }

    /// <summary>
    /// Detecting clicks boils down to defining the user input model, how we detect
    /// click events based on timing, gestures, or locality. This method uses locality,
    /// detecting a click when the product of the x-stdev and y-stdev of mouse inputs
    /// of the last k points falls below some threshold. This seems to provide a nice,
    /// focus-based click detector, though a more rigorous or fluid detector is possible.
    /// </summary>
    /// <param name="eyeData"></param>
    /// <returns></returns>
    public bool DetectLeftClick(RingBuffer<EyeDatum> eyeData)
    {
      bool isClicked;
      int activeRadius = 10;
      double triggerThreshold = 1000;
      double stdXyProduct = StDevXyProduct(activeRadius, eyeData);

      //Console.WriteLine("stdevxy:" + stdXyProduct);
      isClicked = stdXyProduct <= triggerThreshold;

      return isClicked;
    }
  }
}
