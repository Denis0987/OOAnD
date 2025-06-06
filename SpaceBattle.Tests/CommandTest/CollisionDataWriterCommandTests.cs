using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class SaveCollisionDataCommandTests
{
    public SaveCollisionDataCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }

    [Fact]
    public void SaveCollisionDataCommand_SavesDataSuccessfully()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        IoC.Resolve<ICommand>("IoC.Register", "Collision.StoragePath",
            new Func<object[], object>(_ => tempPath)).Execute();

        var fileName = "result.txt";
        var testData = new List<int[]> { new[] { 5, 6, 7 }, new[] { 8, 9, 10 } };

        var storeCommand = new SaveCollisionDataCommand(fileName, testData);
        storeCommand.Execute();

        var filePath = Path.Combine(tempPath, fileName);
        var savedLines = File.ReadAllLines(filePath);

        Assert.Equal("5 6 7", savedLines[0]);
        Assert.Equal("8 9 10", savedLines[1]);

        Directory.Delete(tempPath, true);
    }

    [Fact]
    public void SaveCollisionDataCommand_Fails_WhenPathNotRegistered()
    {
        var invalidCommand = new SaveCollisionDataCommand("error.txt", new List<int[]>());
        Assert.Throws<ArgumentException>(() => invalidCommand.Execute());
    }
}