using System;
using System.Collections.Generic;
using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Lib.GameObjects
{
    public class GameObjectRepository : IGameObjectRepository
    {
        private readonly Dictionary<string, IGameObject> _storage = new();

        public void Add(string id, IGameObject obj)
        {
            if (_storage.ContainsKey(id))
                throw new ArgumentException($"Object '{id}' already exists.");
            _storage[id] = obj;
        }

        public void Remove(string id)
        {
            if (!_storage.ContainsKey(id))
                throw new KeyNotFoundException($"Object '{id}' not found.");
            _storage.Remove(id);
        }

        public bool Contains(string id) => _storage.ContainsKey(id);

        public IGameObject Get(string id)
        {
            if (!_storage.ContainsKey(id))
                throw new KeyNotFoundException($"Object '{id}' not found.");
            return _storage[id];
        }

        public bool Exists(string id) => _storage.ContainsKey(id);

        public void Replace(string id, IGameObject obj)
        {
            if (!_storage.ContainsKey(id))
                throw new KeyNotFoundException($"Object '{id}' not found.");
            _storage[id] = obj;
        }
    }
}
