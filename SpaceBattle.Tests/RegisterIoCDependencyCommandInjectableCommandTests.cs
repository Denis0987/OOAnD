using Hwdtech.Ioc;
using SpaceBattle.Lib;

public class RegisterIoCDependencyCommandInjectableCommandTests
{
    public RegisterIoCDependencyCommandInjectableCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>(
                "Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
            )
            .Execute();
    }

    [Fact]
    public void Resolve_ShouldReturnCommandInjectable()
    {
        var registerCommand = new RegisterIoCDependencyCommandInjectableCommand();
        registerCommand.Execute();

        var resolvedCommand = IoC.Resolve<ICommand>("Commands.CommandInjectable");
        var resolvedCommandInjectable = IoC.Resolve<ICommandInjectable>("Commands.CommandInjectable");
        var resolvedCommandInjectableCommand = IoC.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");

        Assert.NotNull(resolvedCommand);
        Assert.NotNull(resolvedCommandInjectable);
        Assert.NotNull(resolvedCommandInjectableCommand);
        Assert.IsType<CommandInjectableCommand>(resolvedCommand);
        Assert.IsType<CommandInjectableCommand>(resolvedCommandInjectable);
        Assert.IsType<CommandInjectableCommand>(resolvedCommandInjectableCommand);
    }
}
