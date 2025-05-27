namespace SpaceBattle.Lib.Commands
{
    public class RepositoryRemoveCommand : ICommand
    {
        private readonly IDictionary<string, IDictionary<string, object>> _backing;
        private readonly string _uid;

        public RepositoryRemoveCommand(
            IDictionary<string, IDictionary<string, object>> backing,
            string uid)
        {
            _backing = backing ?? throw new ArgumentNullException(nameof(backing));
            _uid = uid;
        }

        public void Execute()
        {
            if (string.IsNullOrWhiteSpace(_uid) || !_backing.ContainsKey(_uid))
            {
                throw new KeyNotFoundException($"Cannot remove: entry '{_uid ?? "<null>"}' not found.");
            }

            _backing.Remove(_uid);
        }
    }
}
