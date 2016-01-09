using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand.EventSystem
{
  public enum EventTypes
  {
    EXPLOSION = 0,
    SPAWN_MISSILE,
    SPAWN_BOMBER,
    COLLISION,
    OUT_OF_AMMO,
    TURRET_SHOT,
    CITY_DESTROYED,
    KEYBOARD_INPUT,
    MOUSE_INPUT,
    AIR_BURST,
    SPAWN_MIRV
    // ... etc
  }
}
