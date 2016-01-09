using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
namespace MissileCommand.Input
{
  //This is a completely generic ring buffer implementation; it could be global scope.
  public class RingBuffer<T>
  {
    T[] _ringBuffer;
    int _curOffset;
    //number of items written to buffer; this will quickly top out at the size of the ring, but is necessary at startup
    int _count;

    public RingBuffer(int size)
    {
      _ringBuffer = new T[size];
      //the offset grows down like a stack, since the buffer keeps most recent data at front
      _curOffset = _ringBuffer.Length - 1;
    }

    public int Count
    {
      get { return _count; }
    }

    /// <summary>
    /// Returns the private physical index corresponding to a public logical offset.
    /// </summary>
    /// <param name="logicalIndex"></param>
    /// <returns></returns>
    int _getPhysicalIndex(int logicalIndex)
    {
      return (_curOffset + logicalIndex) % _ringBuffer.Length;
    }

    //overload of array index operator
    public T this[int logicalIndex]
    {
      get
      {
        return _ringBuffer[_getPhysicalIndex(logicalIndex)]; 
      }
      private set 
      {
        _ringBuffer[_getPhysicalIndex(logicalIndex)] = value;
      }
    }

    public void Push(T newObject)
    {
      //set current index to location to which newObject will be written
      _curOffset--;
      if (_curOffset < 0)
      {
        //wrap the index
        _curOffset = _ringBuffer.Length - 1;
      }

      //only increment count while buffer fills up; once buffer is full, count is just the length of buffer, since we have no Pop() method
      if (_count < _ringBuffer.Length)
      {
        _count++;
      }

      _ringBuffer[_curOffset] = newObject;
    }
  }
}
