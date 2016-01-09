using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand
{
  public interface IGameSpriteContainer
  {
    void Add(IGameSprite newSprite);
    void Remove(IGameSprite oldSprite);
  }
}
