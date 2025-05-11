using SpaceBattle.Lib.Interfaces;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Commands
{
    public class RegisterGameObjectCommand : ICommand
    {
        private readonly string _id;
        private readonly IGameObject _obj;

        public RegisterGameObjectCommand(string id, IGameObject obj)
        {
            _id = id;
            _obj = obj;
        }

        public void Execute()
        {
            var repo = IoC.Resolve<IGameObjectRepository>(
                typeof(IGameObjectRepository).FullName!
            );
            repo.Add(_id, _obj);
        }
    }
}
