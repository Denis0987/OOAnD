using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;
using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Lib.Tests
{
    public class ShotCommandTests
    {
        [Fact]
        public void Execute_Should_Invoke_InitializeProjectile()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var root = IoC.Resolve<object>("Scopes.Root");
            var scope = IoC.Resolve<object>("Scopes.New", root);
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            var shooterMock = new Mock<IShooting>();
            var projectileMock = new Mock<IShooting>().Object;

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Game.Projectile.Create",
                (object[] args) => projectileMock
            ).Execute();

            var initProjectileMock = new Mock<ICommand>();
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Game.Commands.InitializeProjectile",
                (object[] args) => initProjectileMock.Object
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Actions.Start",
                (object[] args) => new Mock<ICommand>().Object
            ).Execute();

            var shotCmd = new ShotCommand(shooterMock.Object);
            shotCmd.Execute();

            initProjectileMock.Verify(c => c.Execute(), Times.Once());
        }

        [Fact]
        public void Execute_Throws_When_Projectile_Not_Registered()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var root = IoC.Resolve<object>("Scopes.Root");
            var scope = IoC.Resolve<object>("Scopes.New", root);
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            var shotCmd = new ShotCommand(new Mock<IShooting>().Object);
            Assert.Throws<ArgumentException>(() => shotCmd.Execute());
        }

        [Fact]
        public void Execute_Throws_When_InitializeProjectile_Not_Registered()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var root = IoC.Resolve<object>("Scopes.Root");
            var scope = IoC.Resolve<object>("Scopes.New", root);
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            var shooterMock = new Mock<IShooting>();
            var projectileObj = new Mock<IShooting>().Object;

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Game.Projectile.Create",
                (object[] args) => projectileObj
            ).Execute();

            var shotCmd = new ShotCommand(shooterMock.Object);
            Assert.Throws<ArgumentException>(() => shotCmd.Execute());
        }

        [Fact]
        public void Execute_Throws_When_StartAction_Not_Registered()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var rootScope = IoC.Resolve<object>("Scopes.Root");
            var scope = IoC.Resolve<object>("Scopes.New", rootScope);
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            var shooter = new Mock<IShooting>();
            var proj = new Mock<IShooting>().Object;

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Game.Projectile.Create",
                (object[] args) => proj
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Game.Commands.InitializeProjectile",
                (object[] args) => new Mock<ICommand>().Object
            ).Execute();

            var shotCmd = new ShotCommand(shooter.Object);
            Assert.Throws<ArgumentException>(() => shotCmd.Execute());
        }
    }
}
