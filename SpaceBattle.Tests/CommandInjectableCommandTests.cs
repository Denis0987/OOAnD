using SpaceBattle.Lib;

public class CommandInjectableCommandTests
{
    [Fact]
    public void Execute_ShouldCallInjectedCommand()
    {
        var mockCommand = new Mock<ICommand>();
        var commandInjectable = new CommandInjectableCommand();

        commandInjectable.Inject(mockCommand.Object);

        commandInjectable.Execute();

        mockCommand.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void Execute_ShouldThrowException_WhenCommandNotInjected()
    {
        var commandInjectable = new CommandInjectableCommand();

        Assert.Throws<InvalidOperationException>(() => commandInjectable.Execute());
    }
}
