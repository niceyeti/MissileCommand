using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand
{
  //convenience class for encapsulating static, global game parameters
  //TODO: figure out the scope and location for this class
  public class GameParameters
  {
    //static public int TOP_BAR_PIXELS = 15;
    //static public int BORDER_PIXELS = 5;
    static public double REFRESH_RATE_MS;
    static public int LEVEL_DURATION_MS;
    static public double DETONATION_PROXIMITY;
    static public int MISSILE_DAMAGE;
    static public int AA_DAMAGE;
    static public int NUM_CITIES;
    static public int MAX_X;
    static public int MAX_Y;
    static public double MIRV_DEPLOY_ALTITUDE;
    static public double MAX_AIR_BURST_RADIUS;
    static public double MISSILE_EXPLOSION_INTENSITY;
    static public double AIR_BURST_INTENSITY;
    static public double MAX_MISSILE_EXPLOSION_RADIUS;
    static public int MISSILE_EXPLOSION_DURATION_MS;
    // the modulus of random noise for missile detonation. this allows creating random explosions local to some fixed point.
    static public int AIR_BURST_DURATION_MS;
    static public int MISSILE_DETONATION_FUZZ_FACTOR;
    static public int EXPLOSION_DAMAGE;
    static public int AIR_BURST_DAMAGE;
    static public int BOMBER_VELOCITY_FUZZ;
    static public double BOMBER_VELOCITY;
    static public int BOMBER_ALTITUDE_FUZZ;
    static public int BOMBER_AMMO;
    static public double BOMB_THETA_FUZZ;
    //Obnoxiously, game/window views often define the top left corner as the origin,
    //with y value increasing down, and x increasing right. These values allow for translating
    //a position so the models can depend on an intuitive bottom-left cartesian origin. Only the
    //view will then do the translation to correct for its own geometric dyslexia.
    static public int VIEW_X_TRANSLATION;
    static public int VIEW_Y_TRANSLATION;

    static public int MIN_X;
    static public int MIN_Y;
    static public double GROUND_LEVEL;
    static public int BOMBER_ALTITUDE;
    static public double MISSILE_VELOCITY;
    static public double MISSILE_ACCELERATION;
    static public double TURRET_SHOT_VELOCITY;
    static public double TURRET_SHOT_ACCELERATION;
    static public double PI;
    //dimension of typical ground items (bases, turrets, bombers)
    static public int GROUND_SPRITE_WIDTH;
    static public int GROUND_SPRITE_HEIGHT;
    static public int GROUND_SPRITE_SPACING;
    static public int MAX_WIDTH;
    static public int MISSILE_WIDTH;
    static public int MISSILE_HEIGHT;
    static public int CITY_BOUNDING_RADIUS;
    static public int CITY_HEALTH;
    static public int TURRET_HEALTH;
    static public int TURRET_AMMO;
    //this is arbitrary, but a formal heuristic ought to be roughly double the radius of the largest object's radius, beyond which no two objects could ever interact.
    static public int MIN_SEPARATION_HEURISTIC_VALUE;
    static public int CITY_HULL_RADIUS;
    static public int TURRET_HULL_RADIUS;
    static public int MISSILE_HULL_RADIUS;
    static public int BOMBER_HULL_RADIUS;
    static public int RETICLE_RADIUS;


    public static void Initialize(int viewableWidth, int viewableHeight)
    {
      REFRESH_RATE_MS = 17; //60 Hz, about what monogame claims it gives
      LEVEL_DURATION_MS = 20000;
      DETONATION_PROXIMITY = 15.0;
      MISSILE_DAMAGE = 0;
      AIR_BURST_INTENSITY = 150;
      MISSILE_EXPLOSION_INTENSITY = 150;
      MISSILE_EXPLOSION_DURATION_MS = 2500; // 2.5 seconds
      AIR_BURST_DURATION_MS = 2500;
      AA_DAMAGE = 100;
      NUM_CITIES = 6;
      MAX_X = viewableWidth;
      MAX_Y = viewableHeight;

      MIRV_DEPLOY_ALTITUDE = 0.40 * (double)MAX_Y;

      //Obnoxiously, game/window views often define the top left corner as the origin,
      //with y value increasing down, and x increasing right. These values allow for translating
      //a position so the models can depend on an intuitive bottom-left cartesian origin. Only the
      //view will then do the translation to correct for its own geometric dyslexia.
      VIEW_X_TRANSLATION = 0;
      VIEW_Y_TRANSLATION = MAX_Y;

      RETICLE_RADIUS = 30;

      MIN_X = 0;
      MIN_Y = 0;
      GROUND_LEVEL = (int)(0.01 * (double)MAX_Y);
      BOMBER_ALTITUDE = (int)(0.75 * (double)MAX_Y);
      MISSILE_VELOCITY = 0.75;
      MISSILE_ACCELERATION = 0.0;
      TURRET_SHOT_VELOCITY = MISSILE_VELOCITY * 4.0;
      BOMBER_AMMO = 5;
      BOMBER_VELOCITY = MISSILE_VELOCITY * 1.5;
      BOMBER_VELOCITY_FUZZ = 1;
      BOMBER_ALTITUDE_FUZZ = MAX_Y / 6;
      BOMB_THETA_FUZZ = Math.PI / 6; // Bomb drop fuzz factor is a 30 degree cone (in rad)
      TURRET_SHOT_ACCELERATION = 3.0;
      PI = Math.PI;
      //dimension of typical ground items (bases, turrets, bombers)
      GROUND_SPRITE_WIDTH = (int)(0.05 * (double)MAX_X);
      GROUND_SPRITE_HEIGHT = (int)(0.04 * (double)MAX_X);
      GROUND_SPRITE_SPACING = (int)(0.50 * (double)GROUND_SPRITE_WIDTH);
      MAX_WIDTH = GROUND_SPRITE_WIDTH;
      MISSILE_WIDTH = 3;
      MISSILE_HEIGHT = 3;
      MAX_MISSILE_EXPLOSION_RADIUS = GROUND_SPRITE_WIDTH * 1.2;
      MAX_AIR_BURST_RADIUS = GROUND_SPRITE_WIDTH * 1.5;
      MISSILE_DETONATION_FUZZ_FACTOR = 20;
      EXPLOSION_DAMAGE = 100; //amount to decrement the health of opposing force objects' health on impact
      AIR_BURST_DAMAGE = 100;
      CITY_HEALTH = 100;
      TURRET_HEALTH = 100;

      TURRET_AMMO = 150;
      //this is arbitrary, but a formal heuristic ought to be roughly double the radius of the largest object's radius, beyond which no two objects could ever interact.
      MIN_SEPARATION_HEURISTIC_VALUE = MAX_WIDTH;
      CITY_HULL_RADIUS = GROUND_SPRITE_WIDTH / 2;
      TURRET_HULL_RADIUS = GROUND_SPRITE_WIDTH / 2;
      MISSILE_HULL_RADIUS = MISSILE_WIDTH / 2;
      BOMBER_HULL_RADIUS = GROUND_SPRITE_WIDTH / 2;
    }

    static GameParameters()
    {

    }
  }
}