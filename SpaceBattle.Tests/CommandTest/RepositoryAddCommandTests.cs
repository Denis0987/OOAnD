using SpaceBattle.Lib.Commands;

namespace SpaceBattle.Tests.Commands
{
    public class RepositoryAddCommandTests
    {
        [Fact]
        public void Execute_UsesProvidedValidUid_WhenNotDuplicate()
        {
            var backing = new Dictionary<string, IDictionary<string, object>>();
            var entry = new Dictionary<string, object> { ["uid"] = "custom-123" };

            new RepositoryAddCommand(backing, entry).Execute();

            Assert.Equal("custom-123", entry["uid"]);
            Assert.Same(entry, backing["custom-123"]);
        }

        [Fact]
        public void Execute_Regenerates_WhenWhitespaceUid()
        {
            var backing = new Dictionary<string, IDictionary<string, object>>();
            var entry = new Dictionary<string, object> { ["uid"] = "   " };

            new RepositoryAddCommand(backing, entry).Execute();

            var newUid = entry["uid"] as string;
            Assert.False(string.IsNullOrWhiteSpace(newUid));
            Assert.True(backing.ContainsKey(newUid!));
        }

        [Fact]
        public void Execute_Regenerates_WhenUidNotString()
        {
            var backing = new Dictionary<string, IDictionary<string, object>>();
            var entry = new Dictionary<string, object> { ["uid"] = 12345 };

            new RepositoryAddCommand(backing, entry).Execute();

            var newUid = entry["uid"]!.ToString();
            Assert.False(string.IsNullOrWhiteSpace(newUid));
            Assert.True(backing.ContainsKey(newUid!));
        }
    }
}
