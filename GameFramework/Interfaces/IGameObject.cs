/* Copyright (c) 2015-2016 Jesse Waite */

using MissileCommand.Kinematics;
using MissileCommand.GameObjects;

namespace MissileCommand.Interfaces
{
  //base class for all drawable objects
  public interface IGameObject
  {
    //Interface Methods
    //interact with another object (collide, explode, etc.)
    void Interact(IGameObject other);
    //time-based update of internal object state to time t + 1 (after interactions have been executed)
    void Update();
    //check if another object is a friend
    bool IsFriend(IGameObject other);
    /// <summary>
    /// Returns the minimum hull distance between an object and some other. Place two spheres next to
    /// eachother: the min hull distance is the distance between their nearest points.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    double HullDistance(IGameObject other);

    ObjectType MyType{get; set;}
    bool PersistAfterDead { get; set; }
    bool StateChanged { get; set; }
    int Health { get; set; }
    int Id { get; set; }
    Position Center { get; set; }
    double HullRadius { get; set; }
    IGameSprite MySprite { get; set; }
    bool IsTransparent { get; set; }
  }
}