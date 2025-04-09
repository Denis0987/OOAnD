namespace SpaceBattle.Lib;

public class RegisterIoCDependencyRotateCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Commands.Rotate",
            (object[] args) => new RotateCommand(IoC.Resolve<IRotating>("Adapters.IRotatingObject", args[0]))
        ).Execute();
    }
}
