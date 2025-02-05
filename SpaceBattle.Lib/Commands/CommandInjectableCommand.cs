namespace SpaceBattle.Lib
{
    public class CommandInjectableCommand : ICommand, ICommandInjectable
    {
        private ICommand? _injectedCommand;

        public void Inject(ICommand command)
        {
            _injectedCommand = command;
        }

        public void Execute()
        {
            if (_injectedCommand == null)
            {
                throw new InvalidOperationException("");
            }

            _injectedCommand.Execute();
        }
    }
}
