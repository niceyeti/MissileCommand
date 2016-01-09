using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using MissileCommand.Kinematics;

namespace MissileCommand.MonoSprites
{
  /// <summary>
  /// MissileSprite extends the SimpleSprite with an original position, and
  /// a draw method that draws a line based on the current position and this
  /// original position.
  /// </summary>
  public class MissileSprite : SimpleSprite
  {
    public Color MissileColor;
    Position _initialPosition;
    int _missileTrailWidth;

    public MissileSprite(Position initialPosition, string resourcePath, int width, int height, SpriteLoaderFlyweight textureFlyweight)
      : base(resourcePath, width, height, textureFlyweight)
    {
      _initialPosition = GameViewFramework.TranslateModelToViewPosition(initialPosition);
      MissileColor = Color.LawnGreen;
      _missileTrailWidth = 1;
      _hasTransparency = false;
    }

    public MissileSprite(Position initialPosition, Color missileColor, string resourcePath, int width, int height, SpriteLoaderFlyweight textureFlyweight)
      : base(resourcePath, width, height, textureFlyweight)
    {
      MissileColor = missileColor;
      _initialPosition = GameViewFramework.TranslateModelToViewPosition(initialPosition);
      _missileTrailWidth = 1;
    }

    public override void Draw(SpriteBatch spriteBatch, ContentManager contentManager)
    {
      if (_isAlive)
      {
        Texture2D texture = _textureFlyweight.LoadTexture2D(this.ResourcePath, contentManager,_hasTransparency);
        Vector2 start = new Vector2((float)_initialPosition.X, (float)_initialPosition.Y);
        Vector2 end = new Vector2((float)this.GetDrawnPosition().X, (float)this.GetDrawnPosition().Y);
        _drawLine(texture, spriteBatch, start, end);
        //draw head of warhead (tiny white square)
        spriteBatch.Draw(texture,new Rectangle((int)this.GetDrawnPosition().X-1,(int)this.GetDrawnPosition().Y-1,_missileTrailWidth+1,_missileTrailWidth+1),Color.WhiteSmoke);
      }
    }

    void _drawLine(Texture2D lineTexture, SpriteBatch sb, Vector2 start, Vector2 end)
    {
      Vector2 edge = end - start;
      // calculate angle to rotate line
      float angle = (float)System.Math.Atan2(edge.Y, edge.X);

      sb.Draw(lineTexture,
              new Rectangle(// rectangle defines shape of line and position of start of line
              (int)start.X,
              (int)start.Y,
              (int)edge.Length(), //sb will strech the texture to fill this rectangle
              _missileTrailWidth), //width of contrail, change this to make thicker line
              null,
              MissileColor, //colour of line
              angle,     //angle of line (calculated above)
              new Vector2(0, 0), // point in line about which to rotate
              SpriteEffects.None,
              0);
    }
  }
}
