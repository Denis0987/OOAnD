using Moq;
using SpaceBattle.Lib;
using Xunit;
using Hwdtech;

public class CommandInjectableCommandTests
{
    [Fact]
    public void Execute_ShouldCallInjectedCommand()
    {
        // Arrange
        var mockCommand = new Mock<SpaceBattle.Lib.ICommand>();
        var commandInjectable = new CommandInjectableCommand();

        commandInjectable.Inject(mockCommand.Object);

        // Act
        commandInjectable.Execute();

        // Assert
        mockCommand.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void Execute_ShouldThrowException_WhenCommandNotInjected()
    {
        // Arrange
        var commandInjectable = new CommandInjectableCommand();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => commandInjectable.Execute());
    }
}