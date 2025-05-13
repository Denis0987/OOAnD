using Hwdtech.Ioc;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests.CommandTests
{
    public class GameTests
    {
        private readonly object _gameScope;
        private readonly Mock<ICommand> _firstCmd;
        private readonly Mock<ICommand> _secondCmd;
        private readonly Mock<ICommand> _errorCmd;
        private readonly Mock<ICommand> _errorHandler;
        private readonly Queue<ICommand> _queue;

        public GameTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var root = IoC.Resolve<object>("Scopes.Root");
            _gameScope = IoC.Resolve<object>("Scopes.New", root);
            IoC.Resolve<ICommand>("Scopes.Current.Set", _gameScope).Execute();

            _firstCmd = new Mock<ICommand>();
            _secondCmd = new Mock<ICommand>();
            _errorCmd = new Mock<ICommand>();
            _errorCmd.Setup(c => c.Execute()).Throws<Exception>();
            _errorHandler = new Mock<ICommand>();

            _queue = new Queue<ICommand>();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Game.Queue.Take",
                (object[] args) => _queue.Dequeue()
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Game.Queue.Count",
                (object[] args) => (object)(() => _queue.Count)
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "ExceptionHandler",
                (object[] args) => _errorHandler.Object
            ).Execute();
        }

        [Fact]
        public void ProcessesAllItems_WhenWithinTimeLimit()
        {
            _queue.Enqueue(_firstCmd.Object);
            _queue.Enqueue(_secondCmd.Object);

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Command.Time",
                (object[] args) => (object)TimeSpan.FromMilliseconds(500)
            ).Execute();

            new Game(_gameScope).Execute();

            _firstCmd.Verify(c => c.Execute(), Times.Once);
            _secondCmd.Verify(c => c.Execute(), Times.Once);
        }

        [Fact]
        public void DoesNotProcess_WhenTimeAlreadyElapsed()
        {
            _queue.Enqueue(_firstCmd.Object);

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Command.Time",
                (object[] args) => (object)TimeSpan.FromMilliseconds(-1)
            ).Execute();

            new Game(_gameScope).Execute();

            _firstCmd.Verify(c => c.Execute(), Times.Never);
        }

        [Fact]
        public void CallsExceptionHandler_OnCommandException()
        {
            _queue.Enqueue(_errorCmd.Object);

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Command.Time",
                (object[] args) => (object)TimeSpan.FromMilliseconds(300)
            ).Execute();

            new Game(_gameScope).Execute();

            _errorHandler.Verify(c => c.Execute(), Times.Once);
        }
    }
}
