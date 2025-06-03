namespace SpaceBattle.Lib.Commands
{
    public class RepositoryRemoveCommand : ICommand
    {
        private readonly Dictionary<string, IDictionary<string, object>> _store;
        private readonly string _uid;

        public RepositoryRemoveCommand(
            Dictionary<string, IDictionary<string, object>> store,
            string uid)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _uid = uid ?? throw new ArgumentNullException(nameof(uid));
        }

        public void Execute()
        {
            if (string.IsNullOrWhiteSpace(_uid))
            {
                throw new ArgumentException(
                    "UID cannot be null or whitespace.", nameof(_uid));
            }

            if (!_store.Remove(_uid))
            {
                throw new KeyNotFoundException(
                    $"Entry with uid '{_uid}' not found.");
            }
        }
    }
}
