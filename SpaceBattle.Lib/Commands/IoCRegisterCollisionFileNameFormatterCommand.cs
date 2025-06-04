namespace SpaceBattle.Lib.Commands;
public class IoCRegisterCollisionFileNameFormatterCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.FileNameFormatter",
            (object[] args) =>
            {
                // args[0] и args[1] — две строки-идентификатора
                return $"{args[0]}__{args[1]}.log";
            }
        ).Execute();
    }
}
