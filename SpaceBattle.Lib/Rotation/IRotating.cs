namespace SpaceBattle.Lib;

public interface IRotating
{
    Angle AnglePosition { get; set; }
    Angle RotateVelocity { get; }
}
