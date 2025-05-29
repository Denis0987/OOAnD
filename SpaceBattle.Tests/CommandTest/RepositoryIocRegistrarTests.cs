using SpaceBattle.Lib.Commands;
using Moq;
using System.Collections.Concurrent;
using System;

namespace SpaceBattle.Tests.CommandTest
{
    public class RepositoryIocRegistrarTests
    {
        public RepositoryIocRegistrarTests()
        {
            var root = IoC.Resolve<object>("Scopes.Root");
            var scope = IoC.Resolve<object>("Scopes.New", root);
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        }

        [Fact]
        public void AddCommand_Throws_OnDuplicateUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var uid = Guid.NewGuid().ToString();
            var entry1 = new Dictionary<string, object> { ["name"] = "E1", ["uid"] = uid };
            var entry2 = new Dictionary<string, object> { ["name"] = "E2", ["uid"] = uid };
            var addCmd1 = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry1 });
            addCmd1.Execute();
            var addCmd2 = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry2 });
            Assert.Throws<InvalidOperationException>(() => addCmd2.Execute());
        }

        [Fact]
        public void RemoveCommand_Throws_OnMissingUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var uid = Guid.NewGuid().ToString();
            var removeCmd = IoC.Resolve<ICommand>("Repository.Remove", new object[] { uid });
            Assert.Throws<KeyNotFoundException>(() => removeCmd.Execute());
        }

        [Fact]
        public void AddCommand_Throws_OnNullEntry()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentNullException>(() => IoC.Resolve<ICommand>("Repository.Add", new object[] { null! }));
        }
        
        [Fact]
        public void AddCommand_Throws_OnMissingArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentException>(() => IoC.Resolve<ICommand>("Repository.Add", Array.Empty<object>()));
        }
        
        [Fact]
        public void RemoveCommand_Throws_OnNullArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentNullException>(() => IoC.Resolve<ICommand>("Repository.Remove", null!));
        }
        
        [Fact]
        public void RemoveCommand_Throws_OnMissingArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentException>(() => IoC.Resolve<ICommand>("Repository.Remove", Array.Empty<object>()));
        }
        
        [Fact]
        public void RemoveCommand_Throws_OnNullUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentException>(() => IoC.Resolve<ICommand>("Repository.Remove", new object[] { null! }));
        }
        
        [Fact]
        public void FetchCommand_Throws_OnNullArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentNullException>(() => IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", null!));
        }
        
        [Fact]
        public void FetchCommand_Throws_OnMissingArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentException>(() => IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", Array.Empty<object>()));
        }
        
        [Fact]
        public void FetchCommand_Throws_OnNullUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentException>(() => IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", new object[] { null! }));
        }

        [Fact]
        public void IoC_Fetch_Throws_OnUnknownUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var uid = Guid.NewGuid().ToString();
            Assert.Throws<KeyNotFoundException>(() => IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", new object[] { uid }));
        }

        [Fact]
        public void FetchCommand_Throws_OnEmptyUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentException>(() => IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", new object[] { "   " }));
        }

        [Fact]
        public void FetchCommand_Throws_OnEmptyStringUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentException>(() => IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", new object[] { string.Empty }));
        }

        [Fact]
        public void AddCommand_GeneratesUid_WhenMissing()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var entry = new Dictionary<string, object> { ["name"] = "E1" };
            var addCmd = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry });
            addCmd.Execute();
            Assert.True(entry.ContainsKey("uid"));
            Assert.False(string.IsNullOrWhiteSpace(entry["uid"].ToString()));
        }

        [Fact]
        public void RemoveCommand_Removes_ExistingEntry()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var entry = new Dictionary<string, object> { ["name"] = "E1", ["uid"] = Guid.NewGuid().ToString() };
            var addCmd = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry });
            addCmd.Execute();
            var removeCmd = IoC.Resolve<ICommand>("Repository.Remove", new object[] { entry["uid"] });
            var ex = Record.Exception(() => removeCmd.Execute());
            Assert.Null(ex);
        }

        [Fact]
        public void AddCommand_Throws_OnNullStore()
        {
            Assert.Throws<ArgumentNullException>(() => new RepositoryAddCommand(null!, new Dictionary<string, object>()));
        }

        [Fact]
        public void IoC_Register_AddCommand_Throws_OnNullArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            
            Assert.Throws<ArgumentNullException>(() => 
                IoC.Resolve<ICommand>("Repository.Add", new object[] { null! }));
        }

        [Fact]
        public void IoC_Register_AddCommand_Throws_OnInvalidEntryType()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            
            Assert.Throws<ArgumentException>(() => 
                IoC.Resolve<ICommand>("Repository.Add", new object[] { "not a dictionary" }));
        }

        [Fact]
        public void IoC_Register_RemoveCommand_Throws_OnNullArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            
            Assert.Throws<ArgumentNullException>(() => 
                IoC.Resolve<ICommand>("Repository.Remove", null!));
        }

        [Fact]
        public void IoC_Register_RemoveCommand_Throws_OnEmptyUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            
            Assert.Throws<ArgumentException>(() => 
                IoC.Resolve<ICommand>("Repository.Remove", new object[] { string.Empty }));
        }

        [Fact]
        public void IoC_Register_FetchCommand_Throws_OnNullArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            
            Assert.Throws<ArgumentNullException>(() => 
                IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", null!));
        }

        [Fact]
        public void IoC_Register_FetchCommand_Throws_OnEmptyUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            
            Assert.Throws<ArgumentException>(() => 
                IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", new object[] { string.Empty }));
        }

        [Fact]
        public void RemoveCommand_Throws_OnNullStore()
        {
            Assert.Throws<ArgumentNullException>(() => new RepositoryRemoveCommand(null!, "uid"));
        }

        [Fact]
        public void AddCommand_Throws_OnNullEntryCtor()
        {
            Assert.Throws<ArgumentNullException>(() => new RepositoryAddCommand(new Dictionary<string, IDictionary<string, object>>(), null!));
        }

        [Fact]
        public void FetchCommand_Returns_CorrectEntry_After_Add()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var testUid = Guid.NewGuid().ToString();
            var entry = new Dictionary<string, object> { ["name"] = "TestEntry", ["uid"] = testUid };

            var addCmd = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry });
            addCmd.Execute();

            var fetched = IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", new object[] { testUid });

            Assert.Equal("TestEntry", fetched["name"]);
            Assert.Equal(testUid, fetched["uid"]);
        }

        [Fact]
        public void AddCommand_WithDifferentValueTypes_StoresCorrectly()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var testUid = Guid.NewGuid().ToString();
            var entry = new Dictionary<string, object>
            {
                ["string"] = "test",
                ["int"] = 123,
                ["bool"] = true,
                ["double"] = 123.45,
                ["nullValue"] = null!,
                ["uid"] = testUid
            };

            var addCmd = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry });
            addCmd.Execute();

            var fetched = IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", new object[] { testUid });
            Assert.Equal("test", fetched["string"]);
            Assert.Equal(123, fetched["int"]);
            Assert.True((bool)fetched["bool"]);
            Assert.Equal(123.45, fetched["double"]);
            Assert.Null(fetched["nullValue"]);
        }

        [Fact]
        public void RemoveCommand_WithWhitespaceUid_ThrowsArgumentException()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();

            Assert.Throws<ArgumentException>(() =>
                IoC.Resolve<ICommand>("Repository.Remove", new object[] { "   " }));
        }

        [Fact]
        public void FetchCommand_WithNonExistentUid_ThrowsKeyNotFoundException()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();

            Assert.Throws<KeyNotFoundException>(() =>
                IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", new object[] { "nonexistent" }));
        }

        [Fact]
        public void RemoveCommand_WithDifferentCaseUid_DoesNotRemove()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var testUid = "TestUid123";
            var entry = new Dictionary<string, object> { ["name"] = "TestEntry", ["uid"] = testUid };

            var addCmd = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry });
            addCmd.Execute();

            var removeCmd = IoC.Resolve<ICommand>("Repository.Remove", new object[] { testUid.ToLower() });

            Assert.Throws<KeyNotFoundException>(() => removeCmd.Execute());

            var fetched = IoC.Resolve<IDictionary<string, object>>("Repository.Fetch", new object[] { testUid });
            Assert.Equal("TestEntry", fetched["name"]);
        }

        [Fact]
        public void RemoveCommand_Throws_OnNullUidCtor()
        {
            Assert.Throws<ArgumentNullException>(() => new RepositoryRemoveCommand(new Dictionary<string, IDictionary<string, object>>(), null!));
        }

        [Fact]
        public void AddCommand_Throws_OnIncorrectType()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentException>(() => IoC.Resolve<ICommand>("Repository.Add", new object[] { 123 }));
        }

        [Fact]
        public void AddCommand_Throws_OnNullArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentNullException>(() => IoC.Resolve<ICommand>("Repository.Add", null!));
        }

        [Fact]
        public void AddCommand_Throws_OnEmptyArgs()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentException>(() => IoC.Resolve<ICommand>("Repository.Add", new object[] { }));
        }

        [Fact]
        public void AddCommand_Throws_OnNullEntryArg()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<ArgumentNullException>(() => IoC.Resolve<ICommand>("Repository.Add", new object[] { null! }));
        }

        [Fact]
        public void AddCommand_GeneratesUid_WhenUidIsWhitespace()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var entry = new Dictionary<string, object> { ["name"] = "E1", ["uid"] = "   " };
            var addCmd = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry });
            addCmd.Execute();
            Assert.True(entry.ContainsKey("uid"));
            Assert.False(string.IsNullOrWhiteSpace(entry["uid"].ToString()));
        }

        [Fact]
        public void AddCommand_Handles_NonStringUid()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var entry = new Dictionary<string, object> { ["name"] = "E1", ["uid"] = 12345 };
            var addCmd = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry });
            var ex = Record.Exception(() => addCmd.Execute());
            Assert.True(ex is null || ex is ArgumentException);
        }

        [Fact]
        public void RepositoryIocRegistrar_Execute_Throws_OnSecondCall()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            Assert.Throws<Exception>(() => registrar.Execute());
        }

        [Fact]
        public void Repository_AddRemove_MultipleEntries()
        {
            var registrar = new RepositoryIocRegistrar();
            registrar.Execute();
            var entry1 = new Dictionary<string, object> { ["name"] = "E1", ["uid"] = Guid.NewGuid().ToString() };
            var entry2 = new Dictionary<string, object> { ["name"] = "E2", ["uid"] = Guid.NewGuid().ToString() };
            var addCmd1 = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry1 });
            var addCmd2 = IoC.Resolve<ICommand>("Repository.Add", new object[] { entry2 });
            addCmd1.Execute();
            addCmd2.Execute();
            var removeCmd1 = IoC.Resolve<ICommand>("Repository.Remove", new object[] { entry1["uid"] });
            var removeCmd2 = IoC.Resolve<ICommand>("Repository.Remove", new object[] { entry2["uid"] });
            var ex1 = Record.Exception(() => removeCmd1.Execute());
            Assert.Null(ex1);
        }


    }
}
