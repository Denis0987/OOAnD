using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Lib.Commands
{
    public class ReplaceGameObjectCommand : ICommand
    {
        private readonly string _id;
        private readonly IGameObject _obj;

        public ReplaceGameObjectCommand(string id, IGameObject obj)
        {
            _id = id;
            _obj = obj;
        }

        public void Execute()
        {
            var repo = IoC.Resolve<IGameObjectRepository>(
                typeof(IGameObjectRepository).FullName!
            );

            if (!repo.Contains(_id))
            {
                throw new InvalidOperationException($"Object '{_id}' not found.");
            }

            repo.Replace(_id, _obj);
        }
    }
}
