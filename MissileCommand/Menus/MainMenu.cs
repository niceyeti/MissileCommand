using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MissileCommand.GameObjects;
using MissileCommand.Interfaces;
using MissileCommand.MonoSprites;
using MissileCommand.Kinematics;

namespace MissileCommand.Menus
{
  /// <summary>
  /// The menu prompting for play/quit, with some banner.
  /// </summary>
  class MainMenu : IMenu
  {
    string _texturePath;
    string _bannerFontPath;
    string _buttonFontPath;
    SpriteLoaderFlyweight _textureLoader;
    Rectangle _playButton;
    Rectangle _quitButton;
    bool _isShown;
    Point _clickPosition;
    bool _clickReceived;

    public MainMenu(SpriteLoaderFlyweight textureLoader, string bannerFontPath, string buttonFontPath, string texturePath)
    {
      _isShown = false;
      _texturePath = texturePath;
      _bannerFontPath = bannerFontPath;
      _buttonFontPath = buttonFontPath;
      _textureLoader = textureLoader;
      _clickReceived = false;

      //These are just to define active regions, for receiving input
      int buttonWidth = GameParameters.MAX_X / 7;
      int buttonHeight = GameParameters.MAX_Y / 15;
      _playButton = new Rectangle(GameParameters.MAX_X / 20, (GameParameters.MAX_Y * 7) / 10 ,buttonWidth,buttonHeight);
      _quitButton = new Rectangle((GameParameters.MAX_X / 20) + GameParameters.MAX_X / 2, (GameParameters.MAX_Y * 7) / 10, buttonWidth, buttonHeight);
    }

    /// <summary>
    /// Just draws the game banner, and two play/quit buttons.
    /// </summary>
    /// <param name="contentManager"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch, ContentManager contentManager)
    {
      //draw the main, semi-transparent rectangle overlay of the game
      Texture2D texture = _textureLoader.LoadTexture2D(_texturePath, contentManager, false);
      spriteBatch.Draw(texture, new Rectangle(0, 0, GameParameters.MAX_X, GameParameters.MAX_Y), Color.Gray);

      //Draw the banner
      SpriteFont bannerFont = _textureLoader.LoadFont(_bannerFontPath, contentManager);
      spriteBatch.DrawString(bannerFont, "MISSILE COMMAND", new Vector2(0, 0), Color.Blue);

      //Draw the buttons
      SpriteFont buttonFont = _textureLoader.LoadFont(_buttonFontPath, contentManager);
      //Draw the left "Play" button
      spriteBatch.DrawString(buttonFont, "Play", new Vector2(_playButton.X, _playButton.Y), Color.Blue);
      //Draw the right "Quit" button
      spriteBatch.DrawString(buttonFont, "Quit", new Vector2(_quitButton.X,_quitButton.Y), Color.Blue);
    }

    /// <summary>
    /// Receives click input
    /// </summary>
    /// <param name="clickPosition"></param>
    public void OnClick(Point clickPosition)
    {
      _clickReceived = true;
      _clickPosition = clickPosition;
    }

    public bool IsShown()
    {
      return _isShown;
    }
  }
}
