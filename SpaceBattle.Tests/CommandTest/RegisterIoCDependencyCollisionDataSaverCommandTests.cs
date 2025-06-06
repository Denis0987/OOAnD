using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class RegisterIoCDependencySaveCollisionDataToFileCommandTests
{
    public RegisterIoCDependencySaveCollisionDataToFileCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }

    [Fact]
    public void DataStorageCommand_RegistersCorrectly()
    {
        new RegisterIoCDependencySaveCollisionDataToFileCommand().Execute();

        var storeCommand = IoC.Resolve<ICommand>("Collision.StoreData", "output.txt", new List<int[]>());
        Assert.NotNull(storeCommand);
        Assert.IsType<SaveCollisionDataCommand>(storeCommand);
    }
}
