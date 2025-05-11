using SpaceBattle.Lib.Interfaces;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Commands
{
    public class DeleteGameObjectCommand : ICommand
    {
        private readonly string _id;

        public DeleteGameObjectCommand(string id)
        {
            _id = id;
        }

        public void Execute()
        {
            var repo = IoC.Resolve<IGameObjectRepository>(
                typeof(IGameObjectRepository).FullName!
            );
            repo.Remove(_id);
        }
    }
}
