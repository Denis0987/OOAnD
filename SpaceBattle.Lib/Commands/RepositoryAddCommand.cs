namespace SpaceBattle.Lib.Commands
{
    public class RepositoryAddCommand : ICommand
    {
        private readonly IDictionary<string, IDictionary<string, object>> _backing;
        private readonly IDictionary<string, object> _entry;

        public RepositoryAddCommand(
            IDictionary<string, IDictionary<string, object>> backing,
            IDictionary<string, object> entry)
        {
            _backing = backing ?? throw new ArgumentNullException(nameof(backing));
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }

        public void Execute()
        {
            if (!_entry.TryGetValue("uid", out var rawUid)
                || !(rawUid is string sUid)
                || string.IsNullOrWhiteSpace(sUid)
                || _backing.ContainsKey(sUid))
            {
                sUid = Guid.NewGuid().ToString("N");
                _entry["uid"] = sUid;
            }

            _backing[sUid] = _entry;
        }
    }
}
