using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Lib.GameObjects
{
    public class GameObject : IGameObject
    {
        private readonly Dictionary<string, object?> _data = new();

        public object? this[string key]
        {
            get => _data[key];
            set => _data[key] = value;
        }
    }
}
