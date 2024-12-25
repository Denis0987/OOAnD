namespace SpaceBattle.Lib;

public class StartCommand(IDictionary<string, object> gameObject, string action) : ICommand
{
    public void Execute()
    {
        var command = IoC.Resolve<ICommand>($"Commands.{action}", gameObject);
        var injectable = (ICommand)IoC.Resolve<ICommandInjectable>("Commands.CommandInjectable");
        var commandReceiver = IoC.Resolve<ICommandReceiver>("Game.CommandsReceiver");
        var sendCommand = IoC.Resolve<ICommand>("Commands.Send", injectable, commandReceiver);
        var result = IoC.Resolve<ICommand>($"Macro.{action}", command, sendCommand);
        ((ICommandInjectable)injectable).Inject(result);
        gameObject[$"repeatable{action}"] = injectable;
        var finalCommand = IoC.Resolve<ICommand>("Commands.Send", result, commandReceiver);
        finalCommand.Execute();
    }
}
