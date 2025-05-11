using Hwdtech.Ioc;
using RegisterIoCDependencyCommand = SpaceBattle.Lib.Commands.RegisterIoCDependencyCommand;
using Moq;
using Xunit;
using SpaceBattle.Lib.Commands;
using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Tests.CommandTest;

public class RegisterGameObjectCommandTests
{
    [Fact]
    public void Execute_AddsObjectToRepository()
    {
        // Инициализация IoC scope
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
        // Arrange
        var repoMock = new Mock<IGameObjectRepository>();
        var obj = new Mock<IGameObject>().Object;

        // регистрируем мок в IoC
        IoC.Resolve<ICommand>(
            "IoC.Register",
            typeof(IGameObjectRepository).FullName!,
            new Func<object[], object>(_ => repoMock.Object)
        ).Execute();

        var cmd = new RegisterGameObjectCommand("new-id", obj);

        // Act
        cmd.Execute();

        // Assert
        repoMock.Verify(r => r.Add("new-id", obj), Times.Once);
    }
}
