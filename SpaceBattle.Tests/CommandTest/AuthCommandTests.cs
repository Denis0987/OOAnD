using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;

namespace SpaceBattle.Lib.Tests.CommandTests
{
    public class AuthCommandTests
    {
        public AuthCommandTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var root = IoC.Resolve<object>("Scopes.Root");
            var scope = IoC.Resolve<object>("Scopes.New", root);
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        }

        [Fact]
        public void Execute_DoesNotThrow_WhenCheckIsTrue()
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Authorization.Check",
                (object[] args) => (object)true
            ).Execute();

            var cmd = new AuthCommand("p1", "Move", "ship1");
            cmd.Execute();
        }

        [Fact]
        public void Execute_ThrowsUnauthorized_WhenCheckIsFalse()
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Authorization.Check",
                (object[] args) => (object)false
            ).Execute();

            var cmd = new AuthCommand("p1", "Move", "ship1");
            var ex = Assert.Throws<UnauthorizedAccessException>(() => cmd.Execute());
            Assert.Equal("Игрок не имеет прав совершать действие над этим обьектом", ex.Message);
        }

        [Fact]
        public void Execute_ThrowsArgumentException_WhenCheckNotRegistered()
        {
            var cmd = new AuthCommand("p1", "Move", "ship1");
            Assert.Throws<ArgumentException>(() => cmd.Execute());
        }

        [Fact]
        public void Execute_PassesCorrectParameters()
        {
            string? seenUser = null, seenAct = null, seenRes = null;
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Authorization.Check",
                (object[] args) =>
                {
                    seenUser = (string)args[0];
                    seenAct = (string)args[1];
                    seenRes = (string)args[2];
                    return (object)true;
                }
            ).Execute();

            var cmd = new AuthCommand("p1", "Fire", "ship42");
            cmd.Execute();

            Assert.Equal("p1", seenUser);
            Assert.Equal("Fire", seenAct);
            Assert.Equal("ship42", seenRes);
        }
    }
}
