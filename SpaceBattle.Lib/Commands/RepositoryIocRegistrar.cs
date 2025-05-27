namespace SpaceBattle.Lib.Commands
{
    public class RepositoryIocRegistrar : ICommand
    {
        public void Execute()
        {
            var store = new Dictionary<string, IDictionary<string, object>>();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Repository.Add",
                (object[] args) =>
                {
                    if (args.Length < 1)
                    {
                        throw new ArgumentException("Repository.Add requires (entry)", nameof(args));
                    }

                    var entry = args[0] as IDictionary<string, object>
                        ?? throw new ArgumentException("expected entry dictionary", nameof(args));
                    return new RepositoryAddCommand(store, entry);
                }
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Repository.Remove",
                (object[] args) =>
                {
                    if (args.Length < 1)
                    {
                        throw new ArgumentException("Repository.Remove requires (uid)", nameof(args));
                    }

                    var uid = args[0]?.ToString()
                        ?? throw new ArgumentException("expected uid", nameof(args));
                    return new RepositoryRemoveCommand(store, uid);
                }
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Repository.Fetch",
                (object[] args) =>
                {
                    if (args.Length < 1)
                    {
                        throw new ArgumentException("Repository.Fetch requires (uid)", nameof(args));
                    }

                    var uid = args[0]?.ToString();
                    if (string.IsNullOrWhiteSpace(uid) || !store.TryGetValue(uid, out var entry))
                    {
                        throw new KeyNotFoundException($"Узел с uid '{uid ?? "<null>"}' не найден.");
                    }

                    return entry;
                }
            ).Execute();
        }
    }
}
