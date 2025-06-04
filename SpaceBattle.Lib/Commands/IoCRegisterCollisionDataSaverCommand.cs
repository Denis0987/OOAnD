namespace SpaceBattle.Lib.Commands;

using System.Collections.Generic;

public class IoCRegisterCollisionDataSaverCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.DataSaver",
            (object[] args) =>
            {
                // args[0] — имя файла (string), args[1] — список массивов (IList<int[]>)
                var fileName = (string)args[0];
                var dataList = (IList<int[]>)args[1];
                return new CollisionDataWriterCommand(fileName, dataList);
            }
        ).Execute();
    }
}
