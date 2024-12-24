using Hwdtech.Ioc;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests
{
    public class RegisterDependencySendCommandTests
    {
        private readonly Mock<ICommand> _command;
        private readonly Mock<ICommandReceiver> _receiver;
        private const string SendCommandDependency = "Commands.Send";

        public RegisterDependencySendCommandTests()
        {
            SetupIoCScope();
            _command = new Mock<ICommand>();
            _receiver = new Mock<ICommandReceiver>();
        }

        private void SetupIoCScope()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        }

        [Fact]
        public void DependencyRegistration_CreatesValidSendCommand()
        {
            RegisterAndValidateSendCommand();
        }

        private void RegisterAndValidateSendCommand()
        {
            new RegisterIoCDependencySendCommand().Execute();
            var command = IoC.Resolve<ICommand>(SendCommandDependency, _command.Object, _receiver.Object);
            ValidateCommand(command);
        }

        private void ValidateCommand(ICommand command)
        {
            Assert.NotNull(command);
            Assert.IsType<SendCommand>(command);
        }
    }
}
