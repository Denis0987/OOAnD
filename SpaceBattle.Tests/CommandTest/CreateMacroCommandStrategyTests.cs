using Hwdtech.Ioc;
namespace SpaceBattle.Tests;
using SpaceBattle.Lib;

public class CreateMacroCommandStrategyTests
{
    public CreateMacroCommandStrategyTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void ResolveMacroCommandExecuteAllCommands()
    {
        var command1 = new Mock<ICommand>();
        var command2 = new Mock<ICommand>();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "command1",
            (object[] args) =>
            command1.Object
            ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "command2",
            (object[] args) =>
            command2.Object
            ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Specs.Macro.Test",
            (object[] args) =>
            new[] { "command1", "command2" }
            ).Execute();

        var strategy = new CreateMacroCommandStrategy("Macro.Test");
        var MacroCommand = strategy.Resolve(new object[] { new object(), new object() });

        MacroCommand.Execute();

        command1.Verify(c => c.Execute(), Times.Once);
        command2.Verify(c => c.Execute(), Times.Once);
    }
    [Fact]
    public void ExceptionNotRegisteredCommand()
    {
        var command1 = new Mock<ICommand>();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Specs.Macro.Test",
            (object[] args) =>
            new[] { "command1", "command2" }
            ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "command1",
            (object[] args) =>
            command1.Object
            ).Execute();

        var strategy = new CreateMacroCommandStrategy("Macro.Test");

        Assert.Throws<ArgumentException>(() => strategy.Resolve(new object[] { new object(), new object() }));
    }
    [Fact]
    public void ExceptionNotRegisteredDependency()
    {
        var command1 = new Mock<ICommand>();
        var command2 = new Mock<ICommand>();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "command1",
            (object[] args) =>
            command1.Object
            ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "command2",
            (object[] args) =>
            command2.Object
            ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Specs.Macro.invalidTest",
            (object[] args) =>
            new[] { "command1", "command2" }
            ).Execute();

        var strategy = new CreateMacroCommandStrategy("Macro.Test");

        Assert.Throws<ArgumentException>(() => strategy.Resolve(new object[] { new object(), new object() }));
    }
}
