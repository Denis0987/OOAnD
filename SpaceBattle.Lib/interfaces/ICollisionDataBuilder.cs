namespace SpaceBattle;

public interface ICollisionDataBuilder
{
    IEnumerable<int> BuildCollisionData(
        Vector location1, Vector speed1, string form1,
        Vector location2, Vector speed2, string form2);
}
