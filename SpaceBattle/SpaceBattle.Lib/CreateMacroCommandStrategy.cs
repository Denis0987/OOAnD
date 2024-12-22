namespace SpaceBattle.Lib;
using Hwdtech;
using Hwdtech.Ioc;

public class CreateMacroCommandStrategy
{
    private readonly string commandSpec_;
    public CreateMacroCommandStrategy(string commandSpec)
    {
        commandSpec_ = commandSpec;
    }
    public SpaceBattle.Lib.ICommand Resolve(object[] args)
    {
        var namesofcommands = IoC.Resolve<string[]>($"Specs.{commandSpec_}");;
        var command = namesofcommands.Select(names => IoC.Resolve<SpaceBattle.Lib.ICommand>(names)).ToArray();

        return new MacroCommand(command);
    }
}