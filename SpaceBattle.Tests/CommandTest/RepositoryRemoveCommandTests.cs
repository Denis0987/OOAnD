using SpaceBattle.Lib.Commands;

namespace SpaceBattle.Tests
{
    public class RepositoryRemoveCommandTests
    {
        [Fact]
        public void Ctor_NullStore_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RepositoryRemoveCommand(null!, "any"));
        }

        [Fact]
        public void Ctor_NullUid_ThrowsArgumentNullException()
        {
            var store = new Dictionary<string, IDictionary<string, object>>();
            Assert.Throws<ArgumentNullException>(() =>
                new RepositoryRemoveCommand(store, null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Execute_WhitespaceUid_ThrowsArgumentException(string badUid)
        {
            var store = new Dictionary<string, IDictionary<string, object>>();
            var cmd = new RepositoryRemoveCommand(store, badUid);
            var ex = Assert.Throws<ArgumentException>(() => cmd.Execute());
            Assert.Contains("UID", ex.ParamName, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Execute_NonexistentUid_ThrowsKeyNotFoundException()
        {
            var store = new Dictionary<string, IDictionary<string, object>>();
            var cmd = new RepositoryRemoveCommand(store, "nope");
            Assert.Throws<KeyNotFoundException>(() => cmd.Execute());
        }

        [Fact]
        public void Execute_ExistingUid_RemovesEntry()
        {
            var store = new Dictionary<string, IDictionary<string, object>>();
            var entry = new Dictionary<string, object> { ["uid"] = "k", ["name"] = "X" };
            store["k"] = entry;

            var cmd = new RepositoryRemoveCommand(store, "k");
            cmd.Execute();

            Assert.False(store.ContainsKey("k"));
        }
    }
}
