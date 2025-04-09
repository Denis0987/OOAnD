namespace SpaceBattle.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib;

public class RegisterIoCDependencyActionsStartTests
{
    private readonly Dictionary<string, object> _testObject;
    private const string ActionKey = "Actions.Start";
    private const string MoveCommand = "Move";

    public RegisterIoCDependencyActionsStartTests()
    {
        SetupIoCScope();
        _testObject = new Dictionary<string, object>();
    }

    private void SetupIoCScope()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void DependencyRegistration_CreatesValidStartCommand()
    {
        RegisterAndValidateStartCommand();
    }

    private void RegisterAndValidateStartCommand()
    {
        new RegisterIoCDependencyActionsStart().Execute();
        var command = IoC.Resolve<ICommand>(ActionKey, _testObject, MoveCommand);
        Assert.NotNull(command);
        Assert.IsType<StartCommand>(command);
    }
}
