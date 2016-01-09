using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MissileCommand.Input
{
  interface IReticle
  {
    MouseState GetMouseState();
    void Draw(SpriteBatch spriteBatch, ContentManager contentManager);
  }
}
