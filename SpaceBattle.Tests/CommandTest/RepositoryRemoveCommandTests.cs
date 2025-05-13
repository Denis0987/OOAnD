using SpaceBattle.Lib.Commands;

namespace SpaceBattle.Tests.CommandTests
{
    public class RepositoryRemoveCommandTests
    {
        [Fact]
        public void Execute_RemovesExisting()
        {
            var backing = new Dictionary<string, IDictionary<string, object>>();
            var uid = "to-remove";
            var entry = new Dictionary<string, object> { ["uid"] = uid };
            backing[uid] = entry;

            new RepositoryRemoveCommand(backing, uid).Execute();
            Assert.False(backing.ContainsKey(uid));
        }

        [Fact]
        public void Execute_Throws_WhenMissing()
        {
            var backing = new Dictionary<string, IDictionary<string, object>>();
            var cmd = new RepositoryRemoveCommand(backing, "none");
            Assert.Throws<KeyNotFoundException>(() => cmd.Execute());
        }
    }
}
