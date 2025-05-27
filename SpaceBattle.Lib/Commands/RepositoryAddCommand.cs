namespace SpaceBattle.Lib.Commands
{
    public class RepositoryAddCommand : ICommand
    {
        private readonly Dictionary<string, IDictionary<string, object>> _store;
        private readonly IDictionary<string, object> _entry;

        public RepositoryAddCommand(
            Dictionary<string, IDictionary<string, object>> store,
            IDictionary<string, object> entry)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }

        public void Execute()
        {
            if (!_entry.TryGetValue("uid", out var uidObj)
                || uidObj == null
                || string.IsNullOrWhiteSpace(uidObj.ToString()))
            {
                var newUid = Guid.NewGuid().ToString();
                _entry["uid"] = newUid;
            }
            else
            {
                var existingUid = uidObj.ToString()!;
                if (_store.ContainsKey(existingUid))
                {
                    throw new InvalidOperationException(
                        $"Entry with uid '{existingUid}' already exists.");
                }
            }

            var finalUid = _entry["uid"]!.ToString()!;
            _store[finalUid] = _entry;
        }
    }
}
