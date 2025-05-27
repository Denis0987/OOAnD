using SpaceBattle.Lib.Commands;

namespace SpaceBattle.Tests.CommandTest
{
    public class RepositoryIocRegistrarTests
    {
        [Fact]
        public void Execute_RegistersAddRemoveFetchFactories()
        {
            var registrar = new RepositoryIocRegistrar();

            registrar.Execute();

            var addEntry = new Dictionary<string, object> { ["name"] = "E1", ["uid"] = Guid.NewGuid() };
            var addCmd = IoC.Resolve<ICommand>("Repository.Add", new object[] { addEntry });
            Assert.IsType<RepositoryAddCommand>(addCmd);
            addCmd.Execute();

            var removeCmd = IoC.Resolve<ICommand>("Repository.Remove", new object[] { addEntry["uid"]! });
            Assert.IsType<RepositoryRemoveCommand>(removeCmd);

            var fetched = IoC.Resolve<IDictionary<string, object>>(
                "Repository.Fetch",
                new object[] { addEntry["uid"]! }
            );
            Assert.Same(addEntry, fetched);
        }
    }
}
