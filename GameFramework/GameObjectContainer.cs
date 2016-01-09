using System.Collections.Generic;
using MissileCommand.Interfaces;

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

    public void SetObjects(List<IGameObject> objectList)
    {
      objects.Clear();
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