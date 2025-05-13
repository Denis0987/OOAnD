namespace SpaceBattle.Lib.Commands
{
    public class AuthCommand : ICommand
    {
        private readonly string _userId;
        private readonly string _action;
        private readonly string _resourceId;

        public AuthCommand(string userId, string action, string resourceId)
        {
            _userId = userId;
            _action = action;
            _resourceId = resourceId;
        }

        public void Execute()
        {
            var isAllowed = IoC.Resolve<bool>(
                "Authorization.Check",
                _userId,
                _action,
                _resourceId
            );

            if (!isAllowed)
            {
                throw new UnauthorizedAccessException(
                    "Игрок не имеет прав совершать действие над этим обьектом"
                );
            }
        }
    }
}
