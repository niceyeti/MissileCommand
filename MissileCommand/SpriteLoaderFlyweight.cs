using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace MissileCommand
{
  /*
   Loading textures is exceptionally expensive at run time. This flyweight holds a pool of 
   sprite textures, reference by their resource-path string. It also provides the service
   of making texture white-pixels transparent on load, but note that as a result the textures
   remain this way for their lifetime.
   */
  public class SpriteLoaderFlyweight
  {
    Dictionary<string,Texture2D> _textureDict;
    Dictionary<string, SpriteFont> _fontDict;
    Dictionary<string, SoundEffect> _soundEffectDict;

    public SpriteLoaderFlyweight()
    {
      _textureDict = new Dictionary<string, Texture2D>();
      _fontDict = new Dictionary<string, SpriteFont>();
      _soundEffectDict = new Dictionary<string, SoundEffect>();
    }

    bool _isWhiteish(Color clr)
    {
      //could also sum and compare with 255*3 +/- some tolerance
      return ((int)clr.R + (int)clr.G + (int)clr.B) > 695;
    }

    //alternative method: make sprite .png's such that any region with an (approximately) white value
    //is instead set to Color.Transparent. Thus, use whiteness (255) as a signal for transparency.
    Color[] _alternativeWhiteishMethod(Texture2D texture)
    {
      Color[] data = new Color[texture.Height * texture.Width];
      texture.GetData<Color>(data);

      for (int i = 0; i < data.Length; i++)
      {
        if (_isWhiteish(data[i]))
        {
          data[i] = Color.Transparent;
        }
      }

      return data;
    }

    //This method performs an exact match on white pixels (255,255,255 rgb)
    Color[] _alternativeRgbMethod(Texture2D texture)
    {
      Color[] data = new Color[texture.Height * texture.Width];
      texture.GetData<Color>(data);

      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == Color.White)
        {
          data[i] = Color.Transparent;
        }
      }

      return data;
    }

    /// <summary>
    /// Takes a texture and returns a copy of its Color data with white portions converted to Color.Transparent,
    /// allowing the sprite batch to draw this texture with transparent portions when alpha-blending is enabled.
    /// 
    /// Note this means using whiteness as a signal value for transparency.
    /// TODO: can this be made more efficient? It traverses every pixel of an image.
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    Color[] _getTransparentColorData(Texture2D texture)
    {
      return _alternativeWhiteishMethod(texture);
    }

    /// <summary>
    /// Retrieves or loads a Texture2D corresponding with the passed resourceString.
    /// The texture is kept in storage for the duration of the game; note the benefits
    /// of this over static class variables, which also wreaked havoc on the sprite hierarchy.
    /// </summary>
    /// <param name="resourcePath"></param>
    /// <param name="contentManager"></param>
    public Texture2D LoadTexture2D(string resourcePath, ContentManager contentManager, bool hasTransparency)
    {
      Texture2D texture = null;

      if (_textureDict.ContainsKey(resourcePath))
      {
        texture = _textureDict[resourcePath];
      }
      else
      {
        try
        {
          texture = contentManager.Load<Texture2D>(resourcePath);
          if (hasTransparency)
          {
            System.Console.WriteLine("Call: "+resourcePath);
            Color[] data = _getTransparentColorData(texture);
            texture.SetData<Color>(data);
          }
          _textureDict.Add(resourcePath, texture);
        }
        catch (Exception e)
        {
          System.Console.WriteLine("Exception caught: Texture2D failed to load from "+resourcePath+"\n"+e.Message+"\n"+e.StackTrace);
        }
      }

      return texture;
    }

    public SoundEffect LoadSoundEffect(string resourcePath, ContentManager contentManager)
    {
      SoundEffect sfx = null;

      if (_soundEffectDict.ContainsKey(resourcePath))
      {
        sfx = _soundEffectDict[resourcePath];
      }
      else 
      {
        try
        {
          sfx = contentManager.Load<SoundEffect>(resourcePath);
          _soundEffectDict.Add(resourcePath, sfx);
        }
        catch (Exception e)
        {
          System.Console.WriteLine("Exception caught: sound effect failed to load from "+resourcePath+"\n"+e.Message+"\n"+e.StackTrace);
        }
      }

      return sfx;
    }

    public SpriteFont LoadFont(string resourcePath, ContentManager contentManager)
    {
      SpriteFont font = null;

      if (_fontDict.ContainsKey(resourcePath))
      {
        font = _fontDict[resourcePath];
      }
      else
      {
        try
        {
          font = contentManager.Load<SpriteFont>(resourcePath);
          _fontDict.Add(resourcePath, font);
        }
        catch (Exception e)
        {
          System.Console.WriteLine("Exception caught: Font failed to load from "+resourcePath+"\n"+e.Message+"\n"+e.StackTrace);
        }
      }
      
      return font;
    }
  }
}
