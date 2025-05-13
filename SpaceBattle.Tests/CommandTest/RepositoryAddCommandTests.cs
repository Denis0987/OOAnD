using SpaceBattle.Lib.Commands;

namespace SpaceBattle.Tests.CommandTests
{
    public class RepositoryAddCommandTests
    {
        [Fact]
        public void Execute_AssignsUid_WhenMissing()
        {
            var backing = new Dictionary<string, IDictionary<string, object>>();
            var entry = new Dictionary<string, object> { ["name"] = "ItemX" };

            new RepositoryAddCommand(backing, entry).Execute();

            Assert.True(entry.ContainsKey("uid"));
            var uid = entry["uid"].ToString()!;

            Assert.True(backing.ContainsKey(uid));
            Assert.Same(entry, backing[uid]);
        }

        [Fact]
        public void Execute_Throws_OnDuplicateUid()
        {
            var backing = new Dictionary<string, IDictionary<string, object>>();
            var uid = "dup-1";
            var entry = new Dictionary<string, object> { ["uid"] = uid };
            backing[uid] = entry;

            var cmd = new RepositoryAddCommand(backing, entry);
            Assert.Throws<InvalidOperationException>(() => cmd.Execute());
        }
    }
}
