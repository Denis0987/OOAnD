using Hwdtech.Ioc;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests
{
    public class RegisterIoCDependencyMacroMoveRotateTests
    {
        public RegisterIoCDependencyMacroMoveRotateTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
        }

        [Fact]
        public void Execute_ShouldRegisterMacroMoveAndRotate()
        {
            var iocScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<ICommand>("Scopes.Current.Set", iocScope).Execute();

            var moveSpec = new[] { "MoveCommand1", "MoveCommand2" };
            var rotateSpec = new[] { "RotateCommand1", "RotateCommand2" };

            var moveCommandMocks = moveSpec.Select(cmd =>
            {
                var mock = new Mock<ICommand>();
                IoC.Resolve<ICommand>("IoC.Register", cmd, (object[] args) => mock.Object).Execute();
                return mock;
            }).ToArray();

            var rotateCommandMocks = rotateSpec.Select(cmd =>
            {
                var mock = new Mock<ICommand>();
                IoC.Resolve<ICommand>("IoC.Register", cmd, (object[] args) => mock.Object).Execute();
                return mock;
            }).ToArray();

            IoC.Resolve<ICommand>("IoC.Register", "Specs.Move", (object[] args) => moveSpec).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "Specs.Rotate", (object[] args) => rotateSpec).Execute();

            new RegisterIoCDependencyMacroMoveRotate().Execute();

            var moveMacro = IoC.Resolve<ICommand>("Macro.Move");
            moveMacro.Execute();
            var rotateMacro = IoC.Resolve<ICommand>("Macro.Rotate");
            rotateMacro.Execute();

            foreach (var mock in moveCommandMocks)
            {
                mock.Verify(cmd => cmd.Execute(), Times.Once()); 
            }

            foreach (var mock in rotateCommandMocks)
            {
                mock.Verify(cmd => cmd.Execute(), Times.Once()); 
            }
        }
    }
}