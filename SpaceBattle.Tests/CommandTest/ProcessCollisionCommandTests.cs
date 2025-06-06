using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class ProcessCollisionCommandTests
{
    public ProcessCollisionCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void ProcessCollisionCommand_ExecutesHandler_WhenCollisionOccurs()
    {
        var handlerMock = new Mock<ICommand>();

        IoC.Resolve<ICommand>("IoC.Register", "Collision.IsColliding", (object[] entities) => (object)true).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Collision.HandleImpact", (object[] entities) => handlerMock.Object).Execute();

        new ProcessCollisionCommand(new object(), new object()).Execute();

        handlerMock.Verify(m => m.Execute(), Times.Once());
    }

    [Fact]
    public void ProcessCollisionCommand_SkipsHandler_WhenNoCollision()
    {
        var handlerMock = new Mock<ICommand>();

        IoC.Resolve<ICommand>("IoC.Register", "Collision.IsColliding", (object[] entities) => (object)false).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Collision.HandleImpact", (object[] entities) => handlerMock.Object).Execute();

        new ProcessCollisionCommand(new object(), new object()).Execute();

        handlerMock.Verify(m => m.Execute(), Times.Never());
    }
}
