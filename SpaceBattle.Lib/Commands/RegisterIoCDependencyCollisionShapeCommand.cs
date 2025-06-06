namespace SpaceBattle;

public class RegisterIoCDependencyCollisionShapeCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.FormRecognizer",
            (object[] inputs) => new ShapeRecognizerCommand()
        ).Execute();
    }
}
