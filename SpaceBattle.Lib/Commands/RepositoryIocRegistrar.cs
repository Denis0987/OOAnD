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
                    var entry = (IDictionary<string, object>)args[0]!;
                    return new RepositoryAddCommand(store, entry);
                }
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Repository.Remove",
                (object[] args) =>
                {
                    var key = args[0]!.ToString()!;
                    return new RepositoryRemoveCommand(store, key);
                }
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Repository.Fetch",
                (object[] args) =>
                {
                    var key = args[0]!.ToString()!;
                    if (!store.TryGetValue(key, out var entry))
                    {
                        throw new KeyNotFoundException($"Узел с uid '{key}' не найден.");
                    }

                    return entry;
                }
            ).Execute();
        }
    }
}
