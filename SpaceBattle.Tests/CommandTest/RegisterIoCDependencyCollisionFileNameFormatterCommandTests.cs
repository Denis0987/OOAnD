using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class RegisterIoCDependencyFormatCollisionFileNameCommandTests
{
    public RegisterIoCDependencyFormatCollisionFileNameCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }

    [Fact]
    public void FileNameGeneration_RegistersSuccessfully()
    {
        new RegisterIoCDependencyFormatCollisionFileNameCommand().Execute();

        var generatedName = IoC.Resolve<string>("Collision.GenerateFileName", "item1", "item2");

        Assert.Equal("item1-item2.txt", generatedName);
    }
}