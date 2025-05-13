namespace SpaceBattle.Lib.Commands
{
    public class RegisterIoCDependencyAuthCheck : ICommand
    {
        public void Execute()
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Authorization.Check",
                new Func<object[], object>(AuthResolver)
            ).Execute();
        }

        private object AuthResolver(object[] parameters)
        {
            var userId = (string)parameters[0];
            var action = (string)parameters[1];
            var resourceId = (string)parameters[2];

            var perms = IoC.Resolve<IDictionary<string, IEnumerable<string>>>(
                "Authorization.GetPermissions",
                userId
            );

            if (perms.TryGetValue("*", out _))
            {
                return (object)true;
            }

            if (!perms.TryGetValue(resourceId, out var objPerms))
            {
                return (object)false;
            }

            return (object)(objPerms.Contains("*") || objPerms.Contains(action));
        }
    }
}
