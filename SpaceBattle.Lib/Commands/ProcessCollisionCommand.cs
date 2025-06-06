namespace SpaceBattle.Lib;

public class ProcessCollisionCommand : ICommand
{
    private readonly object _entity1;
    private readonly object _entity2;

    public ProcessCollisionCommand(object entity1, object entity2)
    {
        _entity1 = entity1;
        _entity2 = entity2;
    }

    public void Execute()
    {
        if (IoC.Resolve<bool>("Collision.IsColliding", _entity1, _entity2))
        {
            IoC.Resolve<ICommand>("Collision.HandleImpact", _entity1, _entity2).Execute();
        }
    }
}
