using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;

namespace SpaceBattle.Tests.CommandTests
{
    public class RepositoryIocRegistrarTests
    {
        public RepositoryIocRegistrarTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<ICommand>(
                "Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
            ).Execute();
        }

        [Fact]
        public void AddThenFetch_ReturnsSameObject()
        {
            new RepositoryIocRegistrar().Execute();

            var obj = new Dictionary<string, object> { ["alpha"] = 1 };
            IoC.Resolve<ICommand>("Repository.Add", obj).Execute();
            var fetched = IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", obj["uid"]);

            Assert.Same(obj, fetched);
        }

        [Fact]
        public void FetchMissing_ThrowsException()
        {
            new RepositoryIocRegistrar().Execute();
            Assert.Throws<KeyNotFoundException>(
                () => IoC.Resolve<object>("Repository.Fetch", "absent")
            );
        }

        [Fact]
        public void RemoveThenFetch_ThrowsAfterRemoval()
        {
            new RepositoryIocRegistrar().Execute();

            var obj = new Dictionary<string, object>();
            IoC.Resolve<ICommand>("Repository.Add", obj).Execute();
            var uid = obj["uid"].ToString()!;

            IoC.Resolve<ICommand>("Repository.Remove", uid).Execute();

            Assert.Throws<KeyNotFoundException>(
                () => IoC.Resolve<object>("Repository.Fetch", uid)
            );
        }
    }
}
