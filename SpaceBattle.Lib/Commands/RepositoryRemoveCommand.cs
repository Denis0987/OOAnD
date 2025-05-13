namespace SpaceBattle.Lib.Commands
{
    public class RepositoryRemoveCommand : ICommand
    {
        private readonly IDictionary<string, IDictionary<string, object>> _store;
        private readonly string _uid;

        public RepositoryRemoveCommand(
            IDictionary<string, IDictionary<string, object>> store,
            string uid
        )
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _uid = uid ?? throw new ArgumentNullException(nameof(uid));
        }

        public void Execute()
        {
            if (!_store.Remove(_uid))
            {
                throw new KeyNotFoundException($"Узел с uid '{_uid}' не найден.");
            }
        }
    }
}
