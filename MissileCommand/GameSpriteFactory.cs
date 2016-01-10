/* Copyright (c) 2015-2016 Jesse Waite */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MissileCommand.GameObjects;
using MissileCommand.Kinematics;
using MissileCommand.MonoSprites;

namespace MissileCommand
{
  class ViewSpriteFactory : IGameSpriteFactory
  {
    //these could be read in via cfg Path
    string _missileImgPath = "Images/Missile"; //and each of these could be object configs, not just images
    string _cityImgPath = "Images/City";  //.. since many could encapsulate short pseudo-animations (image sequences, using state patterns)
    string _explosionImgPath = "Images/Explosion";
    string _turretImgPath = "Images/Turret";
    string _bomberImgPath = "Images/Bomber";
    string _airBurstImgPath = "Images/Explosion";
    string _basicFontPath = "Fonts/BasicFont";
    string _explosionSoundPath = "SoundFX/Explosion Sound Effect _ SFX";
    string _airBurstSoundPath = "SoundFX/Explosion Sound Effect _ SFX";

    //reference to some MonoGame space sprite container
    ViewSpriteContainer _spriteContainer;
    //ContentManager _viewContentManager;
    SpriteLoaderFlyweight _textureFlyweight;

    //TODO: inherit from Texture2D, or have it implement an interface implemented by GameSprites
    public ViewSpriteFactory(ViewSpriteContainer spriteContainer, SpriteLoaderFlyweight textureFlyweight)
    {
      _textureFlyweight = textureFlyweight;
      _spriteContainer = spriteContainer;
    }

    /// <summary>
    /// Generic internal builder for the simplest sprite types.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    SimpleSprite _makeSimpleSprite(ObjectType type)
    {
      SimpleSprite sprite = null;

      switch (type)
      { 
        case ObjectType.CITY:
          sprite = new SimpleSprite(_cityImgPath, GameParameters.GROUND_SPRITE_WIDTH, GameParameters.GROUND_SPRITE_HEIGHT, _textureFlyweight);
          break;
        case ObjectType.BOMBER:
          sprite = new SimpleSprite(_bomberImgPath, GameParameters.GROUND_SPRITE_WIDTH, GameParameters.GROUND_SPRITE_HEIGHT, _textureFlyweight);
          break;
      }

      return sprite;
    }

    public IGameSprite MakeMissileSprite(Position initialPosition)
    {
      MissileSprite sprite = new MissileSprite(initialPosition, _missileImgPath, GameParameters.MISSILE_WIDTH, GameParameters.MISSILE_HEIGHT, _textureFlyweight);
      _spriteContainer.Add(sprite);
      return sprite;
    }

    //Mirv sprite is just a different color missile
    public IGameSprite MakeMirvSprite(Position initialPosition)
    {
      MissileSprite sprite = new MissileSprite(initialPosition, Color.YellowGreen, _missileImgPath, GameParameters.MISSILE_WIDTH, GameParameters.MISSILE_HEIGHT,_textureFlyweight);
      _spriteContainer.Add(sprite);

      return sprite;
    }

    /// <summary>
    /// TurretShotSprite is just a different color of missile sprite, for now.
    /// </summary>
    /// <param name="initialPosition"></param>
    /// <returns></returns>
    public IGameSprite MakeTurretShotSprite(Position initialPosition)
    {
      MissileSprite turretShotSprite = new MissileSprite(initialPosition, _missileImgPath, GameParameters.MISSILE_WIDTH, GameParameters.MISSILE_HEIGHT, _textureFlyweight);
      turretShotSprite.MissileColor = Color.BlueViolet;
      _spriteContainer.Add(turretShotSprite);

      return turretShotSprite;
    }

    public IGameSprite MakeExplosionSprite()
    {
      ExplosionSprite sprite = new ExplosionSprite(_explosionImgPath, _explosionSoundPath, (int)GameParameters.MAX_MISSILE_EXPLOSION_RADIUS, (int)GameParameters.MAX_MISSILE_EXPLOSION_RADIUS, _textureFlyweight);
      sprite.HasTransparency = true;
      _spriteContainer.Add(sprite);
      return sprite;
    }

    public IGameSprite MakeAirBurstSprite()
    {
      AirBurstSprite sprite = new AirBurstSprite(_airBurstImgPath, _airBurstSoundPath, (int)GameParameters.MAX_AIR_BURST_RADIUS, (int)GameParameters.MAX_AIR_BURST_RADIUS, _textureFlyweight);
      sprite.HasTransparency = true;
      _spriteContainer.Add(sprite);
      return sprite;
    }

    public IGameSprite MakeCitySprite()
    {
      SimpleSprite sprite = _makeSimpleSprite(ObjectType.CITY);
      sprite.HasTransparency = true;
      _spriteContainer.Add(sprite);
      return sprite;
    }

    public IGameSprite MakeTurretSprite()
    {
      TurretSprite sprite = new TurretSprite(_turretImgPath, _basicFontPath, GameParameters.GROUND_SPRITE_WIDTH, GameParameters.GROUND_SPRITE_HEIGHT, _textureFlyweight);
      _spriteContainer.Add(sprite);
      return sprite;
    }

    public IGameSprite MakeBomberSprite()
    {
      SimpleSprite sprite = _makeSimpleSprite(ObjectType.BOMBER);
      sprite.HasTransparency = true;
      _spriteContainer.Add(sprite);
      return sprite;
    }
  }
}
