/* Copyright (c) 2015-2016 Jesse Waite */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Collections.Generic;
using MissileCommand.Kinematics;
using MissileCommand.Interfaces;
using MissileCommand.Input;
using MissileCommand.Menus;

namespace MissileCommand
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class GameViewFramework : Game, IGameViewFramework
  {
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;
    Texture2D _background;
    GameOverMenu _gameOverMenu;
    LevelMenu _levelMenu;
    MainMenu _mainMenu;
    IMenu _currentMenu;
    //TODO: appropriate place for reticle and other input based sprites? In game model, our in view?
    //The cursor input runs on the view thread, so I'd argue the object belongs here, even though it smells like a GameObject.
    IReticle _reticle;
    MouseState _mouseState;
    ViewSpriteFactory _gameSpriteFactory;
    ViewSpriteContainer _gameSpriteContainer;
    SpriteLoaderFlyweight _textureFlyweight;
    Vector2 _screenDimension;
    //WaitHandle _contentManagerWaitHandle;
    AutoResetEvent _contentManagerWaitHandle;
    // _line;
    bool _mouseLatched = false;

    /// <summary>
    /// Gets the IGameSpriteFactory. 
    /// 
    /// NOTE: Caller will be put to sleep until the ContentManager has been initialized, which seems
    /// to occur outside the scope of any of this code, and is hidden by the MonoGame framework. This
    /// isn't a robust synchronization method (a better constructor pattern is needed), and has
    /// a high risk of deadlock if similar synchronization startup methods propagate.
    /// </summary>
    /// <returns>The IGameSpriteFactory reference for use by the GameFramework, which doesn't want
    /// to know anything about sprite implementations, only their interface.</returns>
    public IGameSpriteFactory GetIGameSpriteFactory()
    {
      //block, requiring ContentManager to be initialized before permitting access to factory
      _contentManagerWaitHandle.WaitOne();

      if (_gameSpriteFactory == null)
      {
        _gameSpriteFactory = new ViewSpriteFactory(_gameSpriteContainer, _textureFlyweight);
        //create the menus, which just persist in the background. These require the sprite factory, so must be done here
        _gameOverMenu = new GameOverMenu(_textureFlyweight, _gameSpriteFactory, "Fonts/BannerFont", "Fonts/ButtonFont");
        _levelMenu = new LevelMenu(_textureFlyweight, _gameSpriteFactory, "Fonts/ScoreFont");
        _mainMenu = new MainMenu(_textureFlyweight, "Fonts/BannerFont", "Fonts/ButtonFont", "Images/MainMenu");
      }

      return _gameSpriteFactory;
    }

    /// <summary>
    /// If user passes usingEyeTracker, then Game will implement eye tracker filtering and logic
    /// upon mouse inputs. If false, the game defaults to reading the mouse-state normally.
    /// </summary>
    /// <param name="usingEyeTracker"></param>
    public GameViewFramework(bool usingEyeTracker)
    {
      _graphics = new GraphicsDeviceManager(this);
      this.IsMouseVisible = false;
      _graphics.PreferredBackBufferWidth = 800; // GameParameters.MAX_X;// +GameParameters.BORDER_PIXELS * 2;
      _graphics.PreferredBackBufferHeight = 480; // GameParameters.MAX_Y;// +GameParameters.TOP_BAR_PIXELS + GameParameters.BORDER_PIXELS; ;
      //_graphics.IsFullScreen = true;
      _graphics.ApplyChanges();

      this.Content.RootDirectory = "Content";
      _gameSpriteContainer = new ViewSpriteContainer();
      _textureFlyweight = new SpriteLoaderFlyweight();
      _screenDimension.X = _graphics.PreferredBackBufferWidth;
      _screenDimension.Y = _graphics.PreferredBackBufferHeight;
      _contentManagerWaitHandle = new AutoResetEvent(true);

      _reticle = new Reticle(usingEyeTracker, "Images/Reticle", _textureFlyweight);
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      // TODO: Add your initialization logic here
      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      base.Initialize();
    }

    /// <summary>
    /// This translates the position given by the game model into the
    /// coordinates of the view, which like most views, is dyslexically
    /// defined with the origin at top left.
    /// </summary>
    /// <param name="modelPosition"></param>
    /// TODO: remove these to elsewhere
    public static Position TranslateModelToViewPosition(Position modelPosition)
    {
      return new Position(modelPosition.X, GameParameters.VIEW_Y_TRANSLATION - modelPosition.Y);//GameParameters.VIEW_Y_TRANSLATION - modelPosition._y);
    }

    public static Position TranslateViewToModelPosition(int x, int y)
    {
      return new Position(x, GameParameters.VIEW_Y_TRANSLATION - y);//GameParameters.VIEW_Y_TRANSLATION - modelPosition._y);
    }

    public Position GetScreenDimension()
    {
      return new Position((double)_screenDimension.X, (double)_screenDimension.Y);
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      //wake up any thread waiting for the ContentManager to initialize
      _contentManagerWaitHandle.Set();

      // TODO: use this.Content to load your game content here
      _background = this.Content.Load<Texture2D>("Images/Background");
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
      this.Content.Unload();
    }

    //public delegate void MouseInputHandler(MouseState mouseState);
    event MouseInputHandler _onMouseEvent;
    public void SetMouseInputHandler(MouseInputHandler onMouseEvent)
    {
      _onMouseEvent = onMouseEvent;
    }

    //public delegate void KeyboardInputHandler(KeyboardState keyboardState);
    event KeyboardInputHandler _onKeyboardEvent;
    public void SetKeyboardInputHandler(KeyboardInputHandler onKeyboardEvent)
    {
      _onKeyboardEvent = onKeyboardEvent;
    }

    /// <summary>
    /// Blocks the calling model thread until menu display completes.
    /// </summary>
    /// <param name="gameObjects"></param>
    /// <param name="score"></param>
    public void OnLevelCompletion(int remainingCities, int remainingAmmo, int score)
    {
      //TODO: lock the current menu
      _currentMenu = _levelMenu;
      //blocks until menu wakes caller
      _levelMenu.Show(remainingCities, remainingAmmo, score);
    }

    /// <summary>
    /// Blocks the calling thread until animation completes, and user selects retry/quit.
    /// </summary>
    /// <param name="finalScore"></param>
    /// <param name="userWon"></param>
    public bool OnGameOver(int finalScore, bool userWon)
    {
      //TODO: lock the current menu
      _currentMenu = _gameOverMenu;
      //blocking
      return _gameOverMenu.Show(finalScore,userWon);
    }

    bool _isActiveRegion(int x, int y)
    {
      return x < _screenDimension.X && y < _screenDimension.Y;
    }

    void _handleUserInput()
    {
      //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      if (Keyboard.GetState().IsKeyDown(Keys.Escape))
      {
        Exit();
      }

      _mouseState = _reticle.GetMouseState();
      if (_mouseState.LeftButton == ButtonState.Pressed && _isActiveRegion(_mouseState.X, _mouseState.Y))
      {
        //_mouseLatch only allows calling mouse event on rising edge of mouse
        if (!_mouseLatched)
        {
          if (_onMouseEvent != null)
          {
            Position translation = TranslateViewToModelPosition(_mouseState.X, _mouseState.Y);
            _onMouseEvent(_mouseState, translation);
          }
          else
          {
            throw new System.Exception("ERROR OnMouseEvent handler not set in GameViewFramework._handleUserInputs()");
          }

          //notify the menu of mouse left-click as well
          if (_currentMenu != null && _currentMenu.IsShown())
          {
            _currentMenu.OnClick(_mouseState.Position);
          }
        }
      }
      _mouseLatched = _mouseState.LeftButton == ButtonState.Pressed;

      KeyboardState keyboardState = Keyboard.GetState();
      if (keyboardState.GetPressedKeys().Length > 0)
      {
        if (_onKeyboardEvent != null)
        {
          _onKeyboardEvent(keyboardState);
        }
        else
        {
          throw new System.Exception("ERROR OnKeyboardEvent handler not set in GameViewFramework._handleUserInputs()");
        }
      }
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      //handle user inputs over callback
      _handleUserInput();
      //sprite housekeeping
      _gameSpriteContainer.Update();

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// 
    /// Note how this model doesn't seem amenable to menus and the like,
    /// since a menu should only need to dra itself once, then wait for user input,
    /// rather than redrawing itself over and over in the background. For now, menus
    /// must obey the same drawing model as the game, and will be passively re-drawn
    /// while waiting for user input.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      // TODO: Add your drawing code here
      GraphicsDevice.Clear(Color.CornflowerBlue);

      _spriteBatch.Begin();

      //draw the background
      _spriteBatch.Draw(_background, new Rectangle(0, 0, (int)_screenDimension.X, (int)_screenDimension.Y), Color.White);

      //iterate and draw each sprite in turn (-the later sprite will overlap the earlier if two share space)
      for (int i = 0; i < _gameSpriteContainer.Size(); i++)
      {
        try
        {
          _gameSpriteContainer.GetAt(i).Draw(_spriteBatch, Content);
        }
        catch (System.Exception e)
        {
          System.Console.WriteLine("Caught exception attempting drawSprite(): " + e.Message);
        }
      }

      if (_currentMenu != null && _currentMenu.IsShown())
      {
        _currentMenu.Draw(_spriteBatch, Content);
      }

      //draw reticle last
      _reticle.Draw(_spriteBatch, Content);
      
      _spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
