using Hwdtech.Ioc;
using RegisterIoCDependencyCommand = SpaceBattle.Lib.Commands.RegisterIoCDependencyCommand;
using Moq;
using Xunit;
using SpaceBattle.Lib.Commands;
using SpaceBattle.Lib.Interfaces;

namespace SpaceBattle.Tests.CommandTest;

public class DeleteGameObjectCommandTests
{
    [Fact]
    public void Execute_RemovesObjectFromRepository()
    {
        // Инициализация IoC scope
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
        // Arrange
        var repoMock = new Mock<IGameObjectRepository>();
        var id = "test-id";
        repoMock.Setup(r => r.Contains(id)).Returns(true);

        // регистрируем мок в IoC
        IoC.Resolve<ICommand>(
            "IoC.Register",
            typeof(IGameObjectRepository).FullName!,
            new Func<object[], object>(_ => repoMock.Object)
        ).Execute();

        var cmd = new DeleteGameObjectCommand(id);

        // Act
        cmd.Execute();

        // Assert
        repoMock.Verify(r => r.Remove(id), Times.Once);
    }
}
