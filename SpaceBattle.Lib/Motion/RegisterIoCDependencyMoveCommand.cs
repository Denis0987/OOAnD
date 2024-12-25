namespace SpaceBattle.Lib;

public class RegisterIoCDependencyMoveCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Commands.Move",
            (object[] args) => new MoveCommand(new MovingAdapter((IDictionary<string, object>)args[0]))
        ).Execute();
    }
}
