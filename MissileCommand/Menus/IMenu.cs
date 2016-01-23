using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MissileCommand.Menus
{
  interface IMenu
  {
    void Draw(SpriteBatch spriteBatch, ContentManager contentManager);
    bool IsShown();
    void OnClick(Point mousePosition);
  }
}
