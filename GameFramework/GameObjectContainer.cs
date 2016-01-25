/* Copyright (c) 2015-2016 Jesse Waite */

using System.Collections.Generic;
using MissileCommand.Interfaces;
using MissileCommand.GameObjects;

namespace MissileCommand
{
  //main object container
  public class GameObjectContainer
  {
    List<IGameObject> objects;
    int idCounter;

    public GameObjectContainer()
    {
      objects = new List<IGameObject>();
      idCounter = 0;
    }

    /// <summary>
    /// Clears all objects in the container, and also notifies their sprites to die.
    /// </summary>
    public void Clear()
    {
      if (objects.Count > 0)
      {
        //TODO: Should this be moved into the object dtors?
        //signal any remaining objects' sprites to die
        GameSpriteUpdateData data = new GameSpriteUpdateData(new Kinematics.Position(), 0.0);
        data.IsAlive = false;
        foreach (IGameObject gameObject in objects)
        {
          gameObject.MySprite.Update(data);
        }
      }

      objects.Clear();
    }

    /// <summary>
    /// Checks if model contains 
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
    public bool ContainsType(ObjectType objectType)
    {
      return objects.Exists(obj => obj.MyType == objectType);
    }

    //Counts the number of remaining objects of a particular type.
    public int CountType(ObjectType type)
    {
      int count = 0;

      foreach (IGameObject gameObject in objects)
      {
        if (gameObject.MyType == type)
        {
          count++;
        }
      }

      return count;
    }

    public void CleanDeadObjects()
    {
      objects.RemoveAll(obj => obj.Health <= 0);
    }

    public void SetObjects(List<IGameObject> objectList)
    {
      this.Clear();
      objects = objectList;
    }

    public int Size()
    {
      return objects.Count;
    }

    public List<IGameObject>.Enumerator GetEnumerator()
    {
      return objects.GetEnumerator();
    }

    public void RemoveObject(IGameObject obj)
    {
      objects.Remove(obj);
    }

    public List<IGameObject> ToList()
    {
      return objects;
    }
    
    public void Add(IGameObject newObject)
    {
      newObject.Id = idCounter;
      idCounter++;
      objects.Add(newObject);
    }

    public void Remove(int i)
    {
      objects.RemoveAt(i);
    }

    public void RemoveById(int id)
    {
      for (int i = 0; i < objects.Count; i++)
      {
        if (objects[i].Id == id) 
        {
          objects.RemoveAt(i);
        }
      }
    }

    public IGameObject Get(int i)
    {
     IGameObject gameObject = null;

      if (i < objects.Count)
      {
       gameObject = objects[i];
      }

      return gameObject;
    }
  }
}