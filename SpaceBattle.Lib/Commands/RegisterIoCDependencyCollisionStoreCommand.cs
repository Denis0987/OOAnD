namespace SpaceBattle.Lib;

public class RegisterIoCDependencyCollisionStoreCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>("IoC.Register", "Collision.WriteData",
            (object[] inputs) => new StoreCollisionDataCommand(
                (string)inputs[0],
                (IList<int[]>)inputs[1]
            )
        ).Execute();
    }
}
