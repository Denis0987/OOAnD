namespace SpaceBattle.Lib;

public class RotateCommand : ICommand
{
    private readonly IRotating obj;

    public RotateCommand(IRotating obj)
    {
        this.obj = obj;
    }
    public void Execute()
    {
        obj.AnglePosition = obj.AnglePosition + obj.RotateVelocity;
    }
}
