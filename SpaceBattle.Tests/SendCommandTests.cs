using Moq;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests
{
    public class SendCommandTests
    {
        private readonly Mock<ICommandReceiver> _messageHandler;
        private readonly Mock<ICommand> _longRunningTask;
        private readonly ICommand _sendCommand;

        public SendCommandTests()
        {
            _messageHandler = new Mock<ICommandReceiver>();
            _longRunningTask = new Mock<ICommand>();
            _sendCommand = new SendCommand(_longRunningTask.Object, _messageHandler.Object);
        }

        [Fact]
        public void SendCommand_Successfully_Transfers_Command()
        {
            _messageHandler.Setup(x => x.Receive(_longRunningTask.Object));
            _sendCommand.Execute();
            _messageHandler.Verify(handler => handler.Receive(_longRunningTask.Object), Times.Once);
        }

        [Fact]
        public void SendCommand_Propagates_Handler_Exception()
        {
            var expectedError = new Exception("Handler failure");
            _messageHandler
                .Setup(handler => handler.Receive(It.IsAny<ICommand>()))
                .Throws(expectedError);

            var actualError = Assert.Throws<Exception>(() => _sendCommand.Execute());
            Assert.Same(expectedError, actualError);
        }
    }
}
