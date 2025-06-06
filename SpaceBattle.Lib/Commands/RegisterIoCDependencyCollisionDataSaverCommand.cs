namespace SpaceBattle.Lib;

public class RegisterIoCDependencySaveCollisionDataToFileCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>("IoC.Register", "Collision.StoreData",
            (object[] inputs) => new SaveCollisionDataCommand(
                (string)inputs[0],
                (IList<int[]>)inputs[1]
            )
        ).Execute();
    }
}
