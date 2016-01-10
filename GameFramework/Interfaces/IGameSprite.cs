/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissileCommand.Kinematics;

namespace MissileCommand
{
  public interface IGameSprite
  {
    void Update(GameSpriteUpdateData data);
    int GetHeight();
    int GetWidth();
  }
}
