using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using MissileCommand.Kinematics;

namespace MissileCommand.MonoSprites
{
  /// <summary>
  /// TurretSprite extends SimpleSprite by adding a text field to display ammo state.
  /// </summary>
  class TurretSprite : SimpleSprite
  {
    string _fontPath;
    string _text;

    public TurretSprite(string resourcePath, string fontPath, int width, int height, SpriteLoaderFlyweight textureFlyweight)
      : base(resourcePath,width,height, textureFlyweight)
    {
      _fontPath = fontPath;
      _text = "0";
      _hasTransparency = true;
    }

    public override void Update(GameSpriteUpdateData data)
    {
      base.Update(data);
      if (data.Data != null)
      {
        _text = data.Data == "0" ? "OUT" : data.Data;
      }
    }

    public override void Draw(SpriteBatch spriteBatch, ContentManager contentManager)
    {
      if (_isAlive)
      {
        Texture2D texture = _textureFlyweight.LoadTexture2D(this.ResourcePath, contentManager, _hasTransparency);
        spriteBatch.Draw(texture, new Rectangle((int)this.GetDrawnPosition().X, (int)this.GetDrawnPosition().Y, this.Width, this.Height), Color.White);

        //draw the sprite text centered at base of turret
        SpriteFont sf = _textureFlyweight.LoadFont(_fontPath, contentManager);
        int drawnX = (int)this.GetDrawnPosition().X + this.Width / 2 - 15;
        int drawnY = (int)this.GetDrawnPosition().Y + this.Height / 2;
        spriteBatch.DrawString(sf, _text, new Vector2(drawnX,drawnY), Color.Black);
      }
    }
  }
}
