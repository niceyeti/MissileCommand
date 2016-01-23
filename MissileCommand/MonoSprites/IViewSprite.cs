/* Copyright (c) 2015-2016 Jesse Waite */

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MissileCommand.MonoSprites
{
  public interface IViewSprite
  {
    void Draw(SpriteBatch spriteBatch, ContentManager contentManager);
    bool IsAlive();
  }
}
