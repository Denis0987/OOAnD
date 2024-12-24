namespace SpaceBattle.Tests;

using System.Diagnostics;
using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib;

public class StopCommandTests
{
    private readonly Mock<ICommandInjectable> _injectableCommand;
    private readonly Dictionary<string, object> _obj;
    private const string CommandType = "Move";
    private const string CommandKey = "Long";

    public StopCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        _injectableCommand = new Mock<ICommandInjectable>();
        _obj = new Dictionary<string, object>();
        InitializeGameObject();
    }

    private void InitializeGameObject()
    {
        _obj[$"{CommandKey}.{CommandType}"] = _injectableCommand.Object;
    }

    [Fact]
    public void Execute_InjectsEmptyCommand()
    {
        var stopCommand = new StopCommand(_obj, CommandType);

        stopCommand.Execute();

        _injectableCommand.Verify(c => c.Inject(It.IsAny<EmptyCommand>()));
    }

    [Fact]
    public void Execute_RemovesCommandFromDictionary()
    {
        var stopCommand = new StopCommand(_obj, CommandType);

        stopCommand.Execute();

        Assert.False(_obj.ContainsKey($"{CommandKey}.{CommandType}"));
    }

    [Fact]
    public void Execute_CompletesQuickly()
    {
        var stopCommand = new StopCommand(_obj, CommandType);
        var timer = Stopwatch.StartNew();

        stopCommand.Execute();

        timer.Stop();
        Assert.True(timer.ElapsedMilliseconds < 50);
    }

    [Fact]
    public void Execute_ThrowsWhenNoCommandFound()
    {
        var emptyObj = new Dictionary<string, object>();
        var stopCommand = new StopCommand(emptyObj, CommandType);

        var exception = Assert.Throws<InvalidOperationException>(() => stopCommand.Execute());
        Assert.Contains("не начат, остановка", exception.Message.ToLower());
    }
}
