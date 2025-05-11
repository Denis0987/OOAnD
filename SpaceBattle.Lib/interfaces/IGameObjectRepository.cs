using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Lib.Interfaces
{
    public interface IGameObjectRepository
    {
        void Add(string id, IGameObject obj);
        void Remove(string id);
        bool Contains(string id);
        void Replace(string id, IGameObject obj);
    }
}
