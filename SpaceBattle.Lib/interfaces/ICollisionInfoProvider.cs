namespace SpaceBattle.Lib;

public interface ICollisionDataProvider
{
    string FirstId { get; }
    string SecondId { get; }
    IList<int[]> GetCollisionData();
}
