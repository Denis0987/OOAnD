namespace SpaceBattle;

public class RegisterIoCDependencyCollisionBuilderCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.DataBuilder",
            (object[] inputs) => new CollisionDataBuilderCommand(
                IoC.Resolve<IShapeRecognizer>("Collision.FormRecognizer")
            )
        ).Execute();
    }
}
