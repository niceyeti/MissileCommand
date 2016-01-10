/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using MissileCommand.Kinematics;

namespace MissileCommand.MonoSprites
{
  /// <summary>
  /// MissileSprite extends the SimpleSprite with an original position, and
  /// a draw method that draws a line based on the current position and this
  /// original position.
  /// </summary>
  public class AirBurstSprite : SimpleSprite
  {
    bool _once;
    double _sizeScalar;
    SoundEffect _sound;
    string _soundFxPath;

    public AirBurstSprite(string resourcePath, string soundFxPath, int width, int height, SpriteLoaderFlyweight textureFlyweight)
      : base(resourcePath, width, height, textureFlyweight)
    {
      _once = true;
      _soundFxPath = soundFxPath;
      _hasTransparency = true;
    }

    public override void Update(GameSpriteUpdateData data)
    {
      base.Update(data);
      _sizeScalar = data.SizeScalar;
    }

    /// <summary>
    /// This is the method for updating the sprite's associate rectangle with some
    /// scaled value, such that the sprite expands/contracts on update.
    /// </summary>
    /// <returns></returns>
    Rectangle _getScaledSprite()
    {
      //int scaledWidth = (int)((double)this.Width * this._sizeScalar);
      //int scaledHeight = (int)((double)this.Height * this._sizeScalar);
      //int leftEdge = (int)this.DrawnPosition._x + (this.Width - scaledWidth) / 2;
      //int topEdge = (int)this.DrawnPosition._y + (this.Height - scaledHeight) / 2;
      int scaledWidth = (int)((double)this.Width * this._sizeScalar);
      int scaledHeight = (int)((double)this.Height * this._sizeScalar);
      int leftEdge = (int)this.ViewCenter.X - scaledWidth / 2;
      int topEdge = (int)this.ViewCenter.Y - scaledHeight / 2;

      return new Rectangle(leftEdge, topEdge, scaledWidth, scaledHeight);
    }

    override public void Draw(SpriteBatch spriteBatch, ContentManager contentManager)
    {
      if (_isAlive)
      {
        Texture2D texture = _textureFlyweight.LoadTexture2D(this.ResourcePath, contentManager, _hasTransparency);
        Rectangle rect = _getScaledSprite();
        spriteBatch.Draw(texture, rect, Color.White);

        //begin sound play on first Draw() call.
        if (_once)
        {
          _once = false;
          try
          {
            _sound = contentManager.Load<SoundEffect>(_soundFxPath);
            SoundEffectInstance instance = _sound.CreateInstance();
            //float volume = 1.0f;
            //float pitch = 0.0f;
            //float pan = 0.0f;
            instance.Play();// or use: _sound.Play(volume, pitch, pan); and alter pan!
          }
          catch (Exception e)
          {
            System.Console.WriteLine("SOUND FAILED TO LOAD:"+e.Message+e.StackTrace);
          }
        }
      }
    }
  }
}
