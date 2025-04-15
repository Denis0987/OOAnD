namespace SpaceBattle.Lib;

public class CreateMacroCommandStrategy
{
    private readonly string _commandSpec;

    public CreateMacroCommandStrategy(string commandSpec)
    {
        _commandSpec = commandSpec;
    }

    public ICommand Resolve(object[] args)
    {
        var names = IoC.Resolve<IEnumerable<string>>($"Specs.{_commandSpec}");

        var commands = names.Select((name, index) =>
        {
            var arg = index < args.Length ? args[index] : null;
            return (ICommand)IoC.Resolve<ICommand>(name, arg);
        }).ToList();

        return new MacroCommand(commands);
    }
}
