namespace SpaceBattle.Lib;

public class RegisterIoCDependencyActionsStart : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Actions.Start",
            (object[] args) => { return new StartCommand((IDictionary<string, object>)args[0], (string)args[1]); }).Execute();
    }
}
