/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.Kinematics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MissileCommand.EventSystem
{
  //encodes and decodes events. this class will probably go away. functions are symmetric but not cross-platform.
  public class EventCodec //singleton
  {
    static EventCodec _encoder = null;

    public static EventCodec Instance
    {
      get
      {
        if (_encoder == null)
        {
          _encoder = new EventCodec();
        }
        return _encoder;
      }
    }

    // Convert an object to a byte array
    public byte[] Serialize<EventType>(EventType airBurst)
    {
      BinaryFormatter bf = new BinaryFormatter();
      using (MemoryStream ms = new MemoryStream())
      {
        bf.Serialize(ms, (object)airBurst);
        return ms.ToArray();
      }
    }

    // Convert a byte array to an Object
    public EventType Deserialize<EventType>(byte[] data)
    {
      using (MemoryStream memStream = new MemoryStream())
      {
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(data, 0, data.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        EventType deserializedEvent = (EventType)binForm.Deserialize(memStream);
        return deserializedEvent;
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public byte[] PositionToBytes(Position pos)
    {
      byte[] x_bytes = BitConverter.GetBytes(pos.X);
      byte[] y_bytes = BitConverter.GetBytes(pos.Y);
      byte[] bytes = new byte[x_bytes.Length + y_bytes.Length];
      //copy the x and y bytes to a single byte array
      System.Buffer.BlockCopy(x_bytes, 0, bytes, 0, x_bytes.Length);
      System.Buffer.BlockCopy(y_bytes, 0, bytes, x_bytes.Length, y_bytes.Length);

      return bytes;
    }

    public Position BytesToPosition(byte[] bytes)
    {
      double x = 0, y = 0;

      x = BitConverter.ToDouble(bytes, 0);
      y = BitConverter.ToDouble(bytes, sizeof(double));

      return new Position(x, y);
    }
  }
}
