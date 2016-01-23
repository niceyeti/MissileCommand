using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MissileCommand.GameObjects;
using MissileCommand.Interfaces;
using MissileCommand.MonoSprites;
using MissileCommand.Kinematics;
using System.Threading;

namespace MissileCommand.Menus
{
  /// <summary>
  /// Game-over menu is just a rectangle containing text, overlaid over the
  /// game. 
  /// 
  /// TODO: The game-over animation should be within its own class, but can be done later.
  /// </summary>
  class GameOverMenu : IMenu
  {
    bool _shown;
    int _userScore;
    bool _userWon;
    double _intensity;
    double _elapsed_ms;
    double _scalar;
    double _maxRadius = ((double)GameParameters.MAX_X) * 0.7;
    DateTime _start;
    double _duration_ms;
    string _resourcePath;
    SpriteLoaderFlyweight _textureLoader;
    SpriteFont _font;
    ViewSpriteFactory _spriteFactory;
    bool _once;
    List<ExplosionSprite> _explosionSprites;
    string _bannerFontPath;
    string _buttonFontPath;
    Point _clickPosition;
    bool _clickReceived;
    bool _runningAnimation;
    bool _getUserInfo;
    AutoResetEvent _waitHandle;
    bool _retry;

    public GameOverMenu(SpriteLoaderFlyweight textureLoader, ViewSpriteFactory spriteFactory, string bannerFontPath, string buttonFontPath)
    {
      _spriteFactory = spriteFactory;
      _explosionSprites = new List<ExplosionSprite>();
      _elapsed_ms = 0;
      _shown = false;
      _once = true;
      _runningAnimation = true;
      _getUserInfo = false;
      _textureLoader = textureLoader;
      _spriteFactory = spriteFactory;
      _bannerFontPath = bannerFontPath;
      _buttonFontPath = buttonFontPath;
      _waitHandle = new AutoResetEvent(true);
      _retry = false;

      _initializeAnimation();
    }

    public bool IsShown()
    {
      return _shown;
    }

    /// <summary>
    /// Shows the GameOverMenu. This function will block the caller until
    /// the game-over menu completes/returns. On waking, the form result is returned,
    /// indicating whether or not the user wishes to retry.
    /// </summary>
    /// <param name="userScore"></param>
    /// <param name="userWon"></param>
    public bool Show(int userScore, bool userWon)
    {
      _userScore = userScore;
      _userWon = userWon;
      _elapsed_ms = 0;
      _start = DateTime.Now;
      _shown = true;

      System.Console.WriteLine("Model blocked, waiting for level completion menu to return...");
      //lastly, block the caller
      _waitHandle.WaitOne();

      return _retry;
    }

    /// <summary>
    /// Initializes the explosion animation, played when game-over first occurs.
    /// </summary>
    void _initializeAnimation()
    {
      int randX, randY;
      GameSpriteUpdateData data;

      //build the sprites, then immediatey update them with initial values
      for (int i = 0; i < 6; i++)
      {
        _explosionSprites.Add(_spriteFactory.MakeAnimationExplosionSprite());
      }

      //initialize a half dozen or so explosions at random points near the middle region of the screen
      for (int i = 0; i < _explosionSprites.Count; i++)
      {
        randX = RandomNumberGenerator.Instance.Rand() % (GameParameters.MAX_X / 3) + GameParameters.MAX_X / 3;
        randY = RandomNumberGenerator.Instance.Rand() % (GameParameters.MAX_Y / 3) + GameParameters.MAX_X / 3;
        data = new GameSpriteUpdateData(new Position(randX, randY), 1.0, true);
        _explosionSprites[i].Update(data);
      }
    }

    /// <returns>a value ranging from 0-1.0 representing the sprite expansion/contraction scalar</returns>
    public double _updateScalar(double elapsed_ms)
    {
      double scalar = 0.0;

      //only update scalar if time remains
      if (elapsed_ms < _duration_ms)
      {
        scalar = Math.Sin(elapsed_ms * (Math.PI / _duration_ms));
      }

      return scalar;
    }

    /// <summary>
    /// Animation terminates after explosions have expired. Returns true only if elapsed time
    /// is less than max explosion duration.
    /// </summary>
    /// <param name="contentManager"></param>
    /// <param name="spriteBatch"></param>
    bool _drawAnimation(SpriteBatch spriteBatch, ContentManager contentManager)
    {
      //update the scale of the explosions
      _scalar = _updateScalar(_elapsed_ms);
      for (int i = 0; i < _explosionSprites.Count; i++)
      {
        GameSpriteUpdateData data = new GameSpriteUpdateData(_explosionSprites[i].ViewCenter, _scalar);
        _explosionSprites[i].Update(data);
      }
      //draw the explosions
      for (int i = 0; i < _explosionSprites.Count; i++)
      {
        _explosionSprites[i].Draw(spriteBatch, contentManager);
      }

      //display the giant game-over banner under the explosions, after they have reached max size. The explosions then contract and reveal the text.
      if (_elapsed_ms > (_duration_ms / 2))
      {
        SpriteFont font = _textureLoader.LoadFont(_bannerFontPath, contentManager);
        spriteBatch.DrawString(font, "GAME", new Vector2(0, 0), Color.Red);
        spriteBatch.DrawString(font, "OVER", new Vector2(0, 200), Color.Red);
      }

      return _elapsed_ms < _duration_ms;
    }

    /// <summary>
    /// Display the current state of the menu.
    /// Sequence: show game-over animation, record initials (send these to game model), 
    /// then finally get retry (or just return to main menu).
    /// </summary>
    /// <param name="contentManager"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch, ContentManager contentManager)
    {
      _elapsed_ms = (DateTime.Now - _start).TotalMilliseconds;

      //draw the main, semi-transparent rectangle overlay of the game
      Texture2D texture = _textureLoader.LoadTexture2D(_resourcePath, contentManager, false);
      spriteBatch.Draw(texture, new Rectangle(0, 0, GameParameters.MAX_X, GameParameters.MAX_Y), Color.Gray);

      //TODO: This statefulness is gross; need to replace with a stateful pattern of some form
      //first run the game-over animation: a bunch of large explosions at random points
      if (_runningAnimation)
      {
        _runningAnimation = _drawAnimation(spriteBatch, contentManager);
        _getUserInfo = !_runningAnimation;
      }
      //get the user's info
      else if (_getUserInfo)
      {
        _getUserInfo = _drawUserInfoMenu();
        _shown = false;
        //wake the caller of Show() (the model)
        _waitHandle.Set();
      }
    }

    void _resetState()
    {
      _runningAnimation = true;
      _getUserInfo = false;
      _shown = false;
      _retry = false;
    }

    public void OnClick(Point clickPosition)
    {
      _clickReceived = true;
      _clickPosition = clickPosition;
    }

    bool _drawUserInfoMenu()
    {
      //TODO: emulate the main menu
      return false;
    }
  }
}
