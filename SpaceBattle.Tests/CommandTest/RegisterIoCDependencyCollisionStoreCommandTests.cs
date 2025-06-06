using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class RegisterIoCDependencyCollisionStoreCommandTests
{
    public RegisterIoCDependencyCollisionStoreCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void CollisionStoreCommand_RegistersCorrectly()
    {
        new RegisterIoCDependencyCollisionStoreCommand().Execute();

        var storeCommand = IoC.Resolve<ICommand>("Collision.WriteData", "output.txt", new List<int[]>());
        Assert.NotNull(storeCommand);
        Assert.IsType<StoreCollisionDataCommand>(storeCommand);
    }
}
