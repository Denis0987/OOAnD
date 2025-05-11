namespace SpaceBattle.Lib.Interfaces
{
    public interface IGameObject
    {
        object? this[string key] { get; set; }
    }
}
