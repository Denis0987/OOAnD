using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;
using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Lib.Tests
{
    public class RegisterIoCDependencyShotCommandTests
    {
        [Fact]
        public void RegisterIoCDependencyShotCommand_Successfully_Registers_Command()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var rootScope = IoC.Resolve<object>("Scopes.Root");
            var newScope = IoC.Resolve<object>("Scopes.New", rootScope);
            IoC.Resolve<ICommand>("Scopes.Current.Set", newScope).Execute();

            var registrar = new RegisterIoCDependencyShotCommand();
            registrar.Execute();

            var shooterStub = new Mock<IShooting>().Object;
            var cmd = IoC.Resolve<ICommand>("Commands.Shot", shooterStub);

            Assert.NotNull(cmd);
            Assert.IsType<ShotCommand>(cmd);
        }

        [Fact]
        public void RegisterIoCDependencyShotCommand_Throws_When_Called_Twice()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var rootScope = IoC.Resolve<object>("Scopes.Root");
            var newScope = IoC.Resolve<object>("Scopes.New", rootScope);
            IoC.Resolve<ICommand>("Scopes.Current.Set", newScope).Execute();

            var registrar = new RegisterIoCDependencyShotCommand();
            registrar.Execute();

            Assert.Throws<Exception>(() => registrar.Execute());
        }
    }
}
