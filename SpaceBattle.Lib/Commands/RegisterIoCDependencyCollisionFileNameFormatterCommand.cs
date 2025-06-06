
public class RegisterIoCDependencyFormatCollisionFileNameCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>("IoC.Register", "Collision.GenerateFileName",
            (object[] parameters) => $"{parameters[0]}-{parameters[1]}.txt"
        ).Execute();
    }
}
