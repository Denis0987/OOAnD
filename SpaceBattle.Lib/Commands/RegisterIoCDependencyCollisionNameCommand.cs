namespace SpaceBattle.Lib;

public class RegisterIoCDependencyCollisionNameCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>("IoC.Register", "Collision.GenerateName",
            (object[] inputs) => $"{inputs[0]}-{inputs[1]}.txt"
        ).Execute();
    }
}
