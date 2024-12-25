namespace SpaceBattle.Tests;
using SpaceBattle.Lib;
using Hwdtech.Ioc;

public class RegisterIoCDependencyMacroCommandTests
{
    public RegisterIoCDependencyMacroCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }

    [Fact]
    public void ExecuteRegisterIoCMacroCommandDependency()
    {
        var command1 = new Mock<ICommand>();
        var command2 = new Mock<ICommand>();
        var command3 = new Mock<ICommand>();

        new RegisterIoCDependencyMacroCommand().Execute();

        var commands = new List<ICommand> { command1.Object, command2.Object, command3.Object };
        var MacroCommand = IoC.Resolve<ICommand>("Commands.Macro", commands);

        MacroCommand.Execute();

        command1.Verify(c => c.Execute(), Times.Once());
        command2.Verify(c => c.Execute(), Times.Once());
        command3.Verify(c => c.Execute(), Times.Once());
    }
}
