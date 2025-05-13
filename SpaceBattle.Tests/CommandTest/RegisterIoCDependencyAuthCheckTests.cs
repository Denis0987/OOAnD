using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;

namespace SpaceBattle.Lib.Tests.CommandTests
{
    public class RegisterIoCDependencyAuthCheckTests
    {
        public RegisterIoCDependencyAuthCheckTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var root = IoC.Resolve<object>("Scopes.Root");
            var scope = IoC.Resolve<object>("Scopes.New", root);
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        }

        [Fact]
        public void Registration_RegistersCheck()
        {
            var perms = new Dictionary<string, IEnumerable<string>> { { "X", new[] { "Do" } } };
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Authorization.GetPermissions",
                (object[] args) => (object)perms
            ).Execute();

            new RegisterIoCDependencyAuthCheck().Execute();

            var allowed = (bool)IoC.Resolve<object>(
                "Authorization.Check", "u1", "Do", "X"
            );
            Assert.True(allowed);
        }

        [Fact]
        public void GetPermissionsWildcard_AllowsAny()
        {
            var perms = new Dictionary<string, IEnumerable<string>> { { "*", new[] { "A" } } };
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Authorization.GetPermissions",
                (object[] args) => (object)perms
            ).Execute();

            new RegisterIoCDependencyAuthCheck().Execute();
            Assert.True((bool)IoC.Resolve<object>("Authorization.Check", "u1", "B", "Any"));
        }

        [Fact]
        public void AbsentResourceId_Denies()
        {
            var perms = new Dictionary<string, IEnumerable<string>> { { "Y", new[] { "Do" } } };
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Authorization.GetPermissions",
                (object[] args) => (object)perms
            ).Execute();

            new RegisterIoCDependencyAuthCheck().Execute();
            Assert.False((bool)IoC.Resolve<object>("Authorization.Check", "u1", "Do", "X"));
        }

        [Fact]
        public void ResourceWildcard_Allows()
        {
            var perms = new Dictionary<string, IEnumerable<string>> { { "X", new[] { "*" } } };
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Authorization.GetPermissions",
                (object[] args) => (object)perms
            ).Execute();

            new RegisterIoCDependencyAuthCheck().Execute();
            Assert.True((bool)IoC.Resolve<object>("Authorization.Check", "u1", "Anything", "X"));
        }
    }
}
