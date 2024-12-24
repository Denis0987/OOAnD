namespace SpaceBattle.Lib;

public class StopCommand(IDictionary<string, object> gameObject, string action) : ICommand
{
    public void Execute()
    {
        var objInjectable = $"Long.{action}";

        if (!gameObject.ContainsKey(objInjectable))
        {
            throw new InvalidOperationException($"{action} не начат, остановка {action} невозможна");
        }

        var injectable = (ICommandInjectable)gameObject[objInjectable];
        injectable.Inject(new EmptyCommand());
        gameObject.Remove(objInjectable);
    }
}
