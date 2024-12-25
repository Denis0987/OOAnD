using Hwdtech.Ioc;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests
{
    public class RegisterIoCDependencyMovingCommandTests
    {
        public RegisterIoCDependencyMovingCommandTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }

        [Fact]
        public void Execute_Should_Register_Move_Command_Dependency()
        {
            var gameObject = new Dictionary<string, object>();

            var moving = new Mock<IMoving>();

            moving.SetupGet(m => m.Position).Returns(new Vector(12, 5));
            moving.SetupGet(m => m.Velocity).Returns(new Vector(-4, 1));

            var cmd = new RegisterIoCDependencyMoveCommand();

            IoC.Resolve<ICommand>("IoC.Register", "Adapters.IMovingObject", (object[] args) => moving.Object).Execute();

            cmd.Execute();

            var moveCommand = IoC.Resolve<ICommand>("Commands.Move", gameObject);
            moveCommand.Execute();

            Assert.IsType<MoveCommand>(moveCommand);
        }
    }
}