namespace SpaceBattle.Lib;

public class MovingAdapter : IMoving
{
    private readonly IDictionary<string, object> _Game_Object;

    public MovingAdapter(IDictionary<string, object> gameObject)
    {
        _Game_Object = gameObject;
    }

    public Vector Position
    {
        get => (Vector)_Game_Object["Position"];
        set => _Game_Object["Position"] = value;
    }

    public Vector Velocity => (Vector)_Game_Object["Velocity"];
}
