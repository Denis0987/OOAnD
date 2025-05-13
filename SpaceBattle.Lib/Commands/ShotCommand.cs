using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Lib.Commands
{
    public class ShotCommand : ICommand
    {
        private readonly IShooting _shooter;

        public ShotCommand(IShooting shooter)
        {
            _shooter = shooter;
        }

        public void Execute()
        {
            var projectile = IoC.Resolve<IShooting>("Game.Projectile.Create");

            var initCmd = IoC.Resolve<ICommand>(
                "Game.Commands.InitializeProjectile",
                projectile,
                _shooter
            );
            initCmd.Execute();

            var moveCmd = IoC.Resolve<ICommand>(
                "Actions.Start",
                projectile,
                "Move"
            );
            moveCmd.Execute();
        }
    }
}
