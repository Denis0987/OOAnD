using SpaceBattle.Lib.Commands;

namespace SpaceBattle.Tests
{
    public class RepositoryAddCommandTests
    {
        [Fact]
        public void Ctor_NullStore_ThrowsArgumentNullException()
        {
            IDictionary<string, object> dummyEntry = new Dictionary<string, object>();
            Assert.Throws<ArgumentNullException>(() =>
                new RepositoryAddCommand(null!, dummyEntry));
        }

        [Fact]
        public void Ctor_NullEntry_ThrowsArgumentNullException()
        {
            var store = new Dictionary<string, IDictionary<string, object>>();
            Assert.Throws<ArgumentNullException>(() =>
                new RepositoryAddCommand(store, null!));
        }

        [Fact]
        public void Execute_NewEntry_WithNoUid_AddsAndGeneratesUid()
        {
            var store = new Dictionary<string, IDictionary<string, object>>();
            var entry = new Dictionary<string, object> { ["name"] = "TestObj" };
            var cmd = new RepositoryAddCommand(store, entry);

            cmd.Execute();

            Assert.True(entry.ContainsKey("uid"));
            var uid = entry["uid"]!.ToString();
            Assert.False(string.IsNullOrWhiteSpace(uid));
            Assert.True(store.ContainsKey(uid!));
            Assert.Same(entry, store[uid!]);
        }

        [Fact]
        public void Execute_WithExistingUid_ThrowsInvalidOperationException()
        {
            var store = new Dictionary<string, IDictionary<string, object>>();
            var entry1 = new Dictionary<string, object> { ["uid"] = "fixed", ["name"] = "A" };
            var entry2 = new Dictionary<string, object> { ["uid"] = "fixed", ["name"] = "B" };
            var cmd1 = new RepositoryAddCommand(store, entry1);
            cmd1.Execute();

            var cmd2 = new RepositoryAddCommand(store, entry2);
            var ex = Assert.Throws<InvalidOperationException>(() => cmd2.Execute());
            Assert.Contains("fixed", ex.Message);
        }
    }
}
