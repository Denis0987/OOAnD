using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class EmptyCommandTest
{
    [Fact]
    public void EmptyCommand_ExecuteDoesNotThrow()
    {
        var command = new EmptyCommand();
        Assert.Null(Record.Exception(() => command.Execute()));
    }
}
