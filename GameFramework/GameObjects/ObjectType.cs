/* Copyright (c) 2015-2016 Jesse Waite */

namespace MissileCommand.GameObjects
{
  public enum ObjectType
  {
    //mirvs are just missiles, so don't add mirv
    MISSILE = 0,
    EXPLOSION,
    CITY,
    TURRET,
    BOMBER,
    TURRET_SHOT,
    CURSOR,
    AIR_BURST,
    DEATH_HEAD,
    RETICLE
  }
}