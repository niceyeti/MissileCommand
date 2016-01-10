/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MissileCommand.MonoSprites;

namespace MissileCommand
{
  /*
   Container for view sprites. Note that this container is the glue between the view
   * and the model, so this is a concurrent data structure. Currently it just uses
   * locking/blocking with a simple mutex.
   * 
   * TODO: convert mutex to lock semantics. Most commentary online prefers lock.
   */
  public class ViewSpriteContainer
  {
    List<IViewSprite> _spriteContainer;
    Mutex _mutex;

    public ViewSpriteContainer()
    {
      _mutex = new Mutex();
      _spriteContainer = new List<IViewSprite>();
    }

    public void Add(IViewSprite newSprite)
    {
      _mutex.WaitOne();
      _spriteContainer.Add(newSprite);
      _mutex.ReleaseMutex();
    }

    /// <summary>
    /// Remove dead sprites from the container.
    /// 
    /// NOTE: THIS CAN ONLY BE CALLED SYNCHRONOUSLY. You will get random exceptions if the
    /// container is being modified by this call, whilst another thread reads the container.
    /// </summary>
    public void Update()
    {
      _mutex.WaitOne();
      _spriteContainer = _spriteContainer.Where(viewSprite => viewSprite.IsAlive()).ToList<IViewSprite>();
      _mutex.ReleaseMutex();
    }

    public void Remove(IViewSprite oldSprite)
    {
      _mutex.WaitOne();
      _spriteContainer.Remove(oldSprite);
      _mutex.ReleaseMutex();
    }

    public int Size()
    {
      _mutex.WaitOne();
      int count = _spriteContainer.Count;
      _mutex.ReleaseMutex();

      return count;
    }

    public IViewSprite GetAt(int i)
    {
      IViewSprite sprite = null;

      if (i < _spriteContainer.Count)
      {
        _mutex.WaitOne();
        sprite = _spriteContainer[i];
        _mutex.ReleaseMutex();
      }

      return sprite;
    }
  }
}
