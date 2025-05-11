using SpaceBattle.Lib.GameObjects;
using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Lib.Commands;

public class RegisterIoCDependencyCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            typeof(IGameObjectRepository).FullName!,
            new Func<object[], object>(_ => new GameObjectRepository())
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            typeof(RegisterGameObjectCommand).FullName!,
            new Func<object[], object>(args => new RegisterGameObjectCommand((string)args[0], (IGameObject)args[1]))
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            typeof(ReplaceGameObjectCommand).FullName!,
            new Func<object[], object>(args => new ReplaceGameObjectCommand((string)args[0], (IGameObject)args[1]))
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            typeof(DeleteGameObjectCommand).FullName!,
            new Func<object[], object>(args => new DeleteGameObjectCommand((string)args[0]))
        ).Execute();
    }
}
