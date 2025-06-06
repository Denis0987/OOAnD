namespace SpaceBattle;

public class CollisionDataBuilderCommand : ICollisionDataBuilder
{
    private readonly IShapeRecognizer _shapeRecognizer;

    public CollisionDataBuilderCommand(IShapeRecognizer shapeRecognizer)
    {
        _shapeRecognizer = shapeRecognizer;
    }

    public IEnumerable<int> BuildCollisionData(
        Vector location1, Vector speed1, string form1,
        Vector location2, Vector speed2, string form2)
    {
        return location1.Coordinates
            .Concat(location2.Coordinates)
            .Concat(speed1.Coordinates)
            .Concat(speed2.Coordinates)
            .Append(_shapeRecognizer.GetFormId(form1))
            .Append(_shapeRecognizer.GetFormId(form2));
    }
}
