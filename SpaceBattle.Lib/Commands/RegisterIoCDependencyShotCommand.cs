using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Lib.Commands
{
    public class RegisterIoCDependencyShotCommand : ICommand
    {
        public void Execute()
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Shot",
                (object[] parameters) =>
                {
                    var shooter = (IShooting)parameters[0];
                    return new ShotCommand(shooter);
                }
            ).Execute();
        }
    }
}
