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
        var nameofcommand = IoC.Resolve<IEnumerable<string>>($"Specs.{_commandSpec}");

        var command = nameofcommand.Select(name => IoC.Resolve<ICommand>(name, new object[] { args[0] })).ToList();

        return new MacroCommand(command);
    }
}
