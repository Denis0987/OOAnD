namespace SpaceBattle.Tests;

using SpaceBattle.Lib;

public class MacroCommandTests
{
    [Fact]
    public void ExecuteAllCommands()
    {
        var command1 = new Mock<ICommand>();
        var command2 = new Mock<ICommand>();

        var commands = new ICommand[] { command1.Object, command2.Object };
        var MacroСommand = new MacroCommand(commands);

        MacroСommand.Execute();

        command1.Verify(c => c.Execute(), Times.Once);
        command2.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void ExceptionStop()
    {
        var command1 = new Mock<ICommand>();
        var command2 = new Mock<ICommand>();
        var command3 = new Mock<ICommand>();
        command2.Setup(c => c.Execute()).Throws<Exception>();

        var commands = new List<ICommand> { command1.Object, command2.Object, command3.Object };
        var MacroСommand = new MacroCommand(commands);

        Assert.Throws<Exception>(() => MacroСommand.Execute());

        command1.Verify(c => c.Execute(), Times.Once);
        command2.Verify(c => c.Execute(), Times.Once);
        command3.Verify(c => c.Execute(), Times.Never);
    }
}
