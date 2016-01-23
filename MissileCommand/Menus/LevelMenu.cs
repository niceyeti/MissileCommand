using System.Collections.Generic;
using MissileCommand.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using System;

namespace MissileCommand.Menus
{
  class LevelMenu : IMenu
  {
    bool _isShown;
    int _levelScore;
    string _scoreFont;
    ViewSpriteFactory _viewSpriteFactory;
    SpriteLoaderFlyweight _textureLoader;
    AutoResetEvent _waitHandle;
    DateTime _startTime;
    int _showtime_ms;

    public LevelMenu(SpriteLoaderFlyweight textureLoader, ViewSpriteFactory spriteFactory, string scoreFont)
    {
      _isShown = false;
      //number of milliseconds for which to show the menu; this will almost certainly go away...
      _showtime_ms = 5000;
      _scoreFont = scoreFont;
      _viewSpriteFactory = spriteFactory;
      _textureLoader = textureLoader;
      _waitHandle = new AutoResetEvent(true);
    }

    public bool IsShown()
    {
      return _isShown;
    }

    public void Show(List<IGameObject> gameObjects, int score)
    {
      _levelScore = score;
      _startTime = DateTime.Now;
      _isShown = true;
      Console.WriteLine("Model blocked, waiting for level completion menu to return...");
      _waitHandle.WaitOne();
    }

    public void Draw(SpriteBatch spriteBatch, ContentManager contentManager)
    {
      int elapsed_ms = (int)(DateTime.Now - _startTime).TotalMilliseconds;

      //display the score (this is raw)
      if (elapsed_ms < _showtime_ms)
      {
        SpriteFont font = contentManager.Load<SpriteFont>(_scoreFont);
        spriteBatch.DrawString(font, "SCORE: "+_levelScore.ToString(), new Vector2(200, 200), Color.Green);
      }
      else
      {
        _isShown = false;
        //wake the caller of Show()
        _waitHandle.Set();
      }
    }

    public void OnClick(Point clickPosition)
    {
      //TODO: Get rid of this

      //do nothing, just pass-through. This function is just an interface requirement, and likely go away
    }
  }
}
