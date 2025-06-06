namespace SpaceBattle.Lib;

public interface ICollisionDataSource
{
    string FirstId { get; }
    string SecondId { get; }
    IList<int[]> GetCollisionData();
}
