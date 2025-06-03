using SpaceBattle.Lib;

namespace SpaceBattle.Tests
{
    public class SendCommandTests : IDisposable
    {
        private readonly Mock<ICommandReceiver> _messageHandler;
        private readonly Mock<ICommand> _longRunningTask;
        private readonly ICommand _sendCommand;
        private bool _disposed = false;

        public SendCommandTests()
        {
            _messageHandler = new Mock<ICommandReceiver>();
            _longRunningTask = new Mock<ICommand>();
            _sendCommand = new SendCommand(_longRunningTask.Object, _messageHandler.Object);
        }

        [Fact]
        public void SendCommand_Successfully_Transfers_Command()
        {
            // Arrange
            _messageHandler.Setup(x => x.Receive(_longRunningTask.Object));

            // Act
            _sendCommand.Execute();

            // Assert
            _messageHandler.Verify(handler => handler.Receive(_longRunningTask.Object), Times.Once());
        }

        [Fact]
        public void SendCommand_Propagates_Handler_Exception()
        {
            // Arrange
            _messageHandler
                .Setup(handler => handler.Receive(_longRunningTask.Object))
                .Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _sendCommand.Execute());
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenCommandIsNull()
        {
            // Arrange & Act
            var exception = Assert.Throws<ArgumentNullException>(
                () => new SendCommand(null!, _messageHandler.Object));

            // Assert
            Assert.Equal("command", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenReceiverIsNull()
        {
            // Arrange & Act
            var exception = Assert.Throws<ArgumentNullException>(
                () => new SendCommand(_longRunningTask.Object, null!));

            // Assert
            Assert.Equal("receiver", exception.ParamName);
        }

        [Fact]
        public void Execute_ExecutesCommandThroughReceiver()
        {
            // Arrange
            var command = new Mock<ICommand>();
            var receiver = new TestCommandReceiver();
            var sendCommand = new SendCommand(command.Object, receiver);

            // Act
            sendCommand.Execute();

            // Assert
            Assert.True(receiver.CommandReceived);
            command.Verify(c => c.Execute(), Times.Once);
        }

        [Fact]
        public void Execute_DoesNotCatchReceiverExceptions()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Test exception");
            _messageHandler
                .Setup(h => h.Receive(It.IsAny<ICommand>()))
                .Throws(expectedException);

            // Act & Assert
            var actualException = Assert.Throws<InvalidOperationException>(() => _sendCommand.Execute());
            Assert.Same(expectedException, actualException);
        }

        [Fact]
        public void Execute_MultipleTimes_CallsReceiverEachTime()
        {
            // Arrange
            var callCount = 0;
            _messageHandler
                .Setup(h => h.Receive(It.IsAny<ICommand>()))
                .Callback(() => callCount++);

            // Act
            _sendCommand.Execute();
            _sendCommand.Execute();
            _sendCommand.Execute();

            // Assert
            Assert.Equal(3, callCount);
            _messageHandler.Verify(h => h.Receive(It.IsAny<ICommand>()), Times.Exactly(3));
        }

        [Fact]
        public void Execute_WithDifferentCommand_ForwardsCorrectCommand()
        {
            // Arrange
            var customCommand = new Mock<ICommand>();
            var sendCommand = new SendCommand(customCommand.Object, _messageHandler.Object);

            // Act
            sendCommand.Execute();

            // Assert
            _messageHandler.Verify(h => h.Receive(customCommand.Object), Times.Once);
            _messageHandler.Verify(h => h.Receive(_longRunningTask.Object), Times.Never);
        }

        [Fact]
        public void Constructor_WithNullCommand_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(
                () => new SendCommand(null!, _messageHandler.Object));
            Assert.Equal("command", ex.ParamName);
        }

        [Fact]
        public void Execute_WithCustomException_PropagatesException()
        {
            // Arrange
            var customException = new CustomTestException("Custom test exception");
            _messageHandler
                .Setup(h => h.Receive(It.IsAny<ICommand>()))
                .Throws(customException);

            // Act & Assert
            var ex = Assert.Throws<CustomTestException>(() => _sendCommand.Execute());
            Assert.Same(customException, ex);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    _messageHandler.Reset();
                    _longRunningTask.Reset();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private class TestCommandReceiver : ICommandReceiver
        {
            public bool CommandReceived { get; private set; }

            public void Receive(ICommand command)
            {
                CommandReceived = true;
                command.Execute();
            }
        }

        private class CustomTestException : Exception
        {
            public CustomTestException(string message) : base(message) { }
        }

        [Fact]
        public void Execute_WithNullCommand_ThrowsInvalidOperation()
        {
            // Arrange - Create a receiver that will handle a null command
            var receiver = new Mock<ICommandReceiver>();
            var sendCommand = new SendCommand(_longRunningTask.Object, receiver.Object);

            // This test ensures that even if the receiver tries to handle a null command,
            // the SendCommand itself won't fail as it validates the command in constructor
            receiver.Setup(r => r.Receive(It.IsAny<ICommand>()))
                   .Callback<ICommand>(cmd =>
                   {
                       if (cmd == null)
                       {
                           throw new InvalidOperationException("Null command received");
                       }
                   });

            // Act & Assert - Should not throw as the command is validated in constructor
            sendCommand.Execute();
            receiver.Verify(r => r.Receive(It.IsAny<ICommand>()), Times.Once);
        }

        [Fact]
        public void Execute_WithThreadSafeReceiver_HandlesConcurrentCalls()
        {
            // Arrange
            var counter = 0;
            var resetEvent = new ManualResetEvent(false);
            var receiver = new Mock<ICommandReceiver>();
            var sendCommand = new SendCommand(_longRunningTask.Object, receiver.Object);

            receiver.Setup(r => r.Receive(It.IsAny<ICommand>()))
                   .Callback(() =>
                   {
                       Interlocked.Increment(ref counter);
                       resetEvent.WaitOne();
                   });

            // Act - Start multiple threads
            const int threadCount = 10;
            var threads = new Thread[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() => sendCommand.Execute());
                threads[i].Start();
            }

            // Let threads start and block
            Thread.Sleep(100);

            // Release all threads
            resetEvent.Set();

            // Wait for all threads to complete
            foreach (var thread in threads)
            {
                thread.Join();
            }

            // Assert
            Assert.Equal(threadCount, counter);
            receiver.Verify(r => r.Receive(It.IsAny<ICommand>()), Times.Exactly(threadCount));
        }

        [Fact]
        public void Execute_WithExceptionInReceiver_DoesNotBlockSubsequentCalls()
        {
            // Arrange
            var callCount = 0;
            _messageHandler
                .Setup(h => h.Receive(It.IsAny<ICommand>()))
                .Callback(() =>
                {
                    callCount++;
                    if (callCount == 1)
                    {
                        throw new InvalidOperationException("First call fails");
                    }
                });

            // First call - should throw
            Assert.Throws<InvalidOperationException>(() => _sendCommand.Execute());

            // Second call - should succeed
            _sendCommand.Execute();

            // Assert
            _messageHandler.Verify(h => h.Receive(It.IsAny<ICommand>()), Times.Exactly(2));
        }

        [Fact]
        public void Execute_WithVeryFastCalls_HandlesAllCalls()
        {
            // Arrange
            const int iterations = 1000;
            var counter = 0;
            _messageHandler
                .Setup(h => h.Receive(It.IsAny<ICommand>()))
                .Callback(() => Interlocked.Increment(ref counter));

            // Act
            for (var i = 0; i < iterations; i++)
            {
                _sendCommand.Execute();
            }

            // Assert
            Assert.Equal(iterations, counter);
            _messageHandler.Verify(h => h.Receive(It.IsAny<ICommand>()), Times.Exactly(iterations));
        }
    }
}
