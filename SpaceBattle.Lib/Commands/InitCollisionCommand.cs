namespace SpaceBattle.Lib;

public class InitCollisionCommand : ICommand
{
    private readonly ICollisionDataSource _dataSource;

    public InitCollisionCommand(ICollisionDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public void Execute()
    {
        var fileName = IoC.Resolve<string>("Collision.GenerateName", _dataSource.FirstId, _dataSource.SecondId);
        var impactData = _dataSource.GetCollisionData();
        IoC.Resolve<ICommand>("Collision.WriteData", fileName, impactData).Execute();
    }
}
