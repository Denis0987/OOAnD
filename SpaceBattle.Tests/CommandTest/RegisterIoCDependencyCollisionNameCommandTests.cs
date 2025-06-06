using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class RegisterIoCDependencyCollisionNameCommandTests
{
    public RegisterIoCDependencyCollisionNameCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void CollisionNameGeneration_RegistersSuccessfully()
    {
        new RegisterIoCDependencyCollisionNameCommand().Execute();

        var generatedName = IoC.Resolve<string>("Collision.GenerateName", "item1", "item2");

        Assert.Equal("item1-item2.txt", generatedName);
    }
}
