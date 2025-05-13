namespace SpaceBattle.Lib.Commands
{
    public class RepositoryAddCommand : ICommand
    {
        private readonly IDictionary<string, IDictionary<string, object>> _store;
        private readonly IDictionary<string, object> _entry;

        public RepositoryAddCommand(
            IDictionary<string, IDictionary<string, object>> store,
            IDictionary<string, object> entry
        )
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }

        public void Execute()
        {
            if (!_entry.TryGetValue("uid", out var rawUid)
                || rawUid is not string uid
                || string.IsNullOrWhiteSpace(uid))
            {
                uid = Guid.NewGuid().ToString();
                _entry["uid"] = uid;
            }

            if (_store.ContainsKey(uid))
            {
                throw new InvalidOperationException($"Узел с uid '{uid}' уже существует.");
            }

            _store[uid] = _entry;
        }
    }
}
