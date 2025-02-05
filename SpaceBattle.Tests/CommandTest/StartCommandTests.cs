namespace SpaceBattle.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using SpaceBattle.Lib;

public class StartCommandTests
{
    private readonly Mock<ICommand> _moveCommand;
    private readonly Mock<ICommandInjectable> _injectableCommand;
    private readonly Mock<ICommandReceiver> _commandReceiver;
    private readonly Dictionary<string, object> _obj;

    public StartCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        _moveCommand = new Mock<ICommand>();
        _injectableCommand = new Mock<ICommand>().As<ICommandInjectable>();
        _commandReceiver = new Mock<ICommandReceiver>();
        _obj = new Dictionary<string, object>();

        RegisterDependencies();
    }

    private void RegisterDependencies()
    {
        IoC.Resolve<ICommand>("IoC.Register", "Commands.Move",
            (object[] args) => _moveCommand.Object).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Commands.CommandInjectable",
            (object[] args) => _injectableCommand.Object).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Macro.Move",
            (object[] args) => _moveCommand.Object).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Game.CommandsReceiver",
            (object[] args) => _commandReceiver.Object).Execute();

        new RegisterIoCDependencySendCommand().Execute();
    }

    [Fact]
    public void Execute_CreatesAndRegistersCommand()
    {
        var startCommand = new StartCommand(_obj, "Move");

        startCommand.Execute();

        Assert.True(_obj.ContainsKey("repeatableMove"));
    }

    [Fact]
    public void Execute_SendsCommandToReceiver()
    {
        var startCommand = new StartCommand(_obj, "Move");

        startCommand.Execute();

        _commandReceiver.Verify(r => r.Receive(It.IsAny<ICommand>()), Times.Once());
    }
}
