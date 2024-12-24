namespace SpaceBattle.Lib;

public class RegisterIoCDependencyActionsStop : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Actions.Stop",
            (object[] args) => { return new StopCommand((IDictionary<string, object>)args[0], (string)args[1]); }).Execute();
    }
}
