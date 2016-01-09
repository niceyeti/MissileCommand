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
  public class SimpleSprite : IGameSprite, IViewSprite
  {
    protected SpriteLoaderFlyweight _textureFlyweight;

    Position _center;
    public Position ViewCenter
    {
      get { return _center; }
      set { _center = value; }
    }

    //drawn position is defined as the upper left corner of the sprite, +-0.5 height/width from center.
    //Note: This assumes center is already defined according to view coordinate scheme, with origin at upper left.
    public Position GetDrawnPosition()
    {
      return new Position(_center.X - (_width / 2), _center.Y - (_height / 2));
    }

    protected bool _hasTransparency;
    public bool HasTransparency
    {
      set { _hasTransparency = value; }
      get { return _hasTransparency; }
    }

    int _height;
    public int Height
    {
      set { _height = value; }
      get { return _height; }
    }

    public int GetHeight()
    {
      return _height;
    }

    int _width;
    public int Width
    {
      get { return _width; }
      set { _width = value; }
    }

    public int GetWidth()
    {
      return _width;
    }

    string _resourcePath;
    public string ResourcePath
    {
      get { return _resourcePath; }
    }

    protected bool _isAlive;
    public bool IsAlive()
    {
      return _isAlive;
    }

    public SimpleSprite(string resourcePath, int width, int height, SpriteLoaderFlyweight textureFlyweight)
    {
      _textureFlyweight = textureFlyweight;
      _resourcePath = resourcePath;
      _center = new Position(0, 0);
      _height = height;
      _width = width;
      _isAlive = true;
    }

    public virtual void Draw(SpriteBatch spriteBatch, ContentManager contentManager)
    {
      if (_isAlive)
      {
        Texture2D texture = _textureFlyweight.LoadTexture2D(this.ResourcePath, contentManager, _hasTransparency);
        spriteBatch.Draw(texture, new Rectangle((int)this.GetDrawnPosition().X, (int)this.GetDrawnPosition().Y, this.Width, this.Height), Color.White);
      }
    }

    public virtual void Update(GameSpriteUpdateData data)
    {
      if (_isAlive)
      {
        _center = GameViewFramework.TranslateModelToViewPosition(data.NewPosition);
        _isAlive = data.IsAlive;
      }
    }
  }
}
