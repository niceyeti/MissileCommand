using MissileCommand.Kinematics;

namespace MissileCommand.EventSystem
{
  //delegate for when a bomb is dropped
  public delegate void BombDropEvent(Particle hostVector);
  //delegate for when an explosive item detonates
  public delegate void DetonationHandler(Particle args);
  //delegate for when a turret takes a shot
  public delegate void TurretShotHandler(Position source, Position target);
}