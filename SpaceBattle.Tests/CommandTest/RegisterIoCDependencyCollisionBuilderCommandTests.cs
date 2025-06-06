using Hwdtech.Ioc;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyCollisionBuilderCommandTests
{
    public RegisterIoCDependencyCollisionBuilderCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        new RegisterIoCDependencyCollisionShapeCommand().Execute();
    }

    [Fact]
    public void RegisterCollisionBuilder_Succeeds()
    {
        new RegisterIoCDependencyCollisionBuilderCommand().Execute();

        var builder = IoC.Resolve<ICollisionDataBuilder>("Collision.DataBuilder");
        Assert.NotNull(builder);
    }
}
