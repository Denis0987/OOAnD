using Hwdtech.Ioc;
using Moq;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests
{
    public class RegisterDependencySendCommandTests
    {
        public RegisterDependencySendCommandTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<ICommand>("Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }

        [Fact]
        public void Execute_RegistersDependency()
        {
            // Arrange
            var command = new Mock<ICommand>();
            var receiver = new Mock<ICommandReceiver>();

            // Act
            new RegisterIoCDependencySendCommand().Execute();

            // Assert
            var resolvedCommand = IoC.Resolve<ICommand>("Commands.Send", command.Object, receiver.Object);
            Assert.NotNull(resolvedCommand);
            Assert.IsType<SendCommand>(resolvedCommand);
        }
    }
}
