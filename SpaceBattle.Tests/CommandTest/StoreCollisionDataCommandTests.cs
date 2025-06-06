using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class StoreCollisionDataCommandTests
{
    public StoreCollisionDataCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void StoreCollisionDataCommand_SavesDataCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        IoC.Resolve<ICommand>("IoC.Register", "Collision.StoragePath",
            new Func<object[], object>(_ => tempPath)).Execute();

        var fileName = "result.txt";
        var testData = new List<int[]> { new[] { 7, 8, 9 }, new[] { 10, 11, 12 } };

        var storeCommand = new StoreCollisionDataCommand(fileName, testData);
        storeCommand.Execute();

        var filePath = Path.Combine(tempPath, fileName);
        var savedLines = File.ReadAllLines(filePath);

        Assert.Equal("7 8 9", savedLines[0]);
        Assert.Equal("10 11 12", savedLines[1]);

        Directory.Delete(tempPath, true);
    }

    [Fact]
    public void StoreCollisionDataCommand_Fails_WhenPathNotRegistered()
    {
        var invalidCommand = new StoreCollisionDataCommand("error.txt", new List<int[]>());
        Assert.Throws<ArgumentException>(() => invalidCommand.Execute());
    }
}
