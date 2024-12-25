using Hwdtech.Ioc;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests
{
    public class RegisterIoCDependencyMoveCommandTests
    {
        public RegisterIoCDependencyMoveCommandTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }

        [Fact]
        public void Execute_Should_Register_Move_Command_Dependency()
        {
            var Game_Object = new Dictionary<string, object>
            {
                { "Position", new Vector(2, 3) },
                { "Velocity", new Vector(3, 2) }
            };

            new RegisterIoCDependencyMoveCommand().Execute();

            var moveCommand = IoC.Resolve<ICommand>("Commands.Move", Game_Object);
            moveCommand.Execute();

            Assert.Equal(new Vector(5, 5), (Vector)Game_Object["Position"]);
        }
    }
}
