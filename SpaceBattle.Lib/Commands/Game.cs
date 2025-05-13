using System.Diagnostics;

namespace SpaceBattle.Lib
{
    public class Game : ICommand
    {
        private readonly object _executionScope;
        private readonly Stopwatch _timer;

        public Game(object executionScope)
        {
            _executionScope = executionScope;
            _timer = new Stopwatch();
        }

        public void Execute()
        {
            _timer.Reset();
            IoC.Resolve<ICommand>("Scopes.Current.Set", _executionScope).Execute();

            var maxDuration = IoC.Resolve<TimeSpan>("Command.Time");
            var getQueueSize = IoC.Resolve<Func<int>>("Game.Queue.Count");

            while (getQueueSize() > 0 && _timer.Elapsed <= maxDuration)
            {
                ProcessNextItem();
            }
        }

        private void ProcessNextItem()
        {
            _timer.Start();
            var item = IoC.Resolve<ICommand>("Game.Queue.Take");
            try
            {
                item.Execute();
            }
            catch (Exception ex)
            {
                IoC.Resolve<ICommand>("ExceptionHandler", ex, item).Execute();
            }
            finally
            {
                _timer.Stop();
            }
        }
    }
}
