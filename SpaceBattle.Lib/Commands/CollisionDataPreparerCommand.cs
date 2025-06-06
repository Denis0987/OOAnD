namespace SpaceBattle.Lib;

public class PrepareCollisionDataCommand : ICommand
{
    private readonly ICollisionDataProvider _dataSource;

    public PrepareCollisionDataCommand(ICollisionDataProvider dataSource)
    {
        _dataSource = dataSource;
    }

    public void Execute()
    {
        var fileName = IoC.Resolve<string>("Collision.GenerateFileName", _dataSource.FirstId, _dataSource.SecondId);
        var impactData = _dataSource.GetCollisionData();
        IoC.Resolve<ICommand>("Collision.StoreData", fileName, impactData).Execute();
    }
}
