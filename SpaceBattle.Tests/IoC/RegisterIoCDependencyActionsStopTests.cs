namespace SpaceBattle.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib;

public class RegisterIoCDependencyActionsStopTests
{
    private readonly Dictionary<string, object> _testObject;
    private const string ActionKey = "Actions.Stop";
    private const string MoveCommand = "Move";

    public RegisterIoCDependencyActionsStopTests()
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
    public void DependencyRegistration_CreatesValidStopCommand()
    {
        RegisterAndValidateStopCommand();
    }

    private void RegisterAndValidateStopCommand()
    {
        new RegisterIoCDependencyActionsStop().Execute();
        var command = IoC.Resolve<ICommand>(ActionKey, _testObject, MoveCommand);
        Assert.NotNull(command);
        Assert.IsType<StopCommand>(command);
    }
}
