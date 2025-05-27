namespace SpaceBattle.Lib.Commands
{
    public class RepositoryIocRegistrar : ICommand
    {
        private readonly Dictionary<string, IDictionary<string, object>> store = new();
        public void Execute()
        {

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Repository.Add",
                (object[] args) =>
                {
                    if (args == null)
                    {
                        throw new ArgumentNullException(nameof(args));
                    }

                    if (args.Length < 1 || args[0] == null)
                    {
                        throw new ArgumentException(
                            "Entry argument cannot be null.", nameof(args));
                    }

                    if (args[0] is not IDictionary<string, object> entry)
                    {
                        throw new ArgumentException(
                            "Invalid entry type.", nameof(args));
                    }

                    return new RepositoryAddCommand(store, entry);
                }
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Repository.Remove",
                (object[] args) =>
                {
                    if (args == null)
                    {
                        throw new ArgumentNullException(nameof(args));
                    }

                    if (args.Length < 1 || args[0] == null)
                    {
                        throw new ArgumentException(
                            "UID argument cannot be null.", nameof(args));
                    }

                    var key = args[0].ToString()!;
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new ArgumentException(
                            "UID cannot be null or whitespace.", nameof(key));
                    }

                    return new RepositoryRemoveCommand(store, key);
                }
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Repository.Fetch",
                (object[] args) =>
                {
                    if (args == null)
                    {
                        throw new ArgumentNullException(nameof(args));
                    }

                    if (args.Length < 1 || args[0] == null)
                    {
                        throw new ArgumentException(
                            "UID argument cannot be null.", nameof(args));
                    }

                    var key = args[0].ToString()!;
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new ArgumentException(
                            "UID cannot be null or whitespace.", nameof(key));
                    }

                    if (!store.TryGetValue(key, out var entry))
                    {
                        throw new KeyNotFoundException(
                            $"Entry with uid '{key}' not found.");
                    }

                    return entry;
                }
            ).Execute();
        }
    }
}
