
using Hwdtech.Ioc;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests
{
    public class RegisterIoCDependencyRotateCommandTests
    {
        public RegisterIoCDependencyRotateCommandTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }

        [Fact]
        public void Execute_Should_Register_Rotate_Command_Dependency()
        {
            var gameObject = new Dictionary<string, object>();

            var rotating = new Mock<IRotating>();

            rotating.SetupGet(x => x.AnglePosition).Returns(new Angle(45, 8));
            rotating.SetupGet(x => x.RotateVelocity).Returns(new Angle(90, 8));

            var cmd = new RegisterIoCDependencyRotateCommand();

            IoC.Resolve<ICommand>("IoC.Register", "Adapters.IRotatingObject", (object[] args) => rotating.Object).Execute();

            cmd.Execute();

            var rotateCommand = IoC.Resolve<ICommand>("Commands.Rotate", gameObject);
            rotateCommand.Execute();

            Assert.IsType<RotateCommand>(rotateCommand);
        }
    }
}
