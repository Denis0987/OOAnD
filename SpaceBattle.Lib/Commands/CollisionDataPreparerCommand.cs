namespace SpaceBattle.Lib.Commands;

using SpaceBattle.Lib.Interfaces;

public class CollisionDataPreparerCommand : ICommand
{
    private readonly ICollisionInfoProvider _provider;

    public CollisionDataPreparerCommand(ICollisionInfoProvider provider)
    {
        _provider = provider;
    }

    public void Execute()
    {
        var generatedName = IoC.Resolve<string>(
            "Collision.FileNameFormatter",
            _provider.FirstObjectId,
            _provider.SecondObjectId);

        var collisionPoints = _provider.GetCollisionPoints();

        IoC.Resolve<ICommand>(
            "Collision.DataSaver",
            generatedName,
            collisionPoints
        ).Execute();
    }
}
