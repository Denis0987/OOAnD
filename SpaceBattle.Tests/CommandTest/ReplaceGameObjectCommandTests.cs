using System;
using Hwdtech.Ioc;
using RegisterIoCDependencyCommand = SpaceBattle.Lib.Commands.RegisterIoCDependencyCommand;
using Moq;
using Xunit;
using SpaceBattle.Lib.Commands;
using SpaceBattle.Lib.Interfaces;
using SpaceBattle.Lib.GameObjects;

namespace SpaceBattle.Tests.CommandTest;

public class ReplaceGameObjectCommandTests
{
    [Fact]
    public void Execute_ReplacesExistingObject()
    {
        // Инициализация IoC scope
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
        // Arrange
        var repoMock = new Mock<IGameObjectRepository>();
        var newObj = new GameObject();
        repoMock.Setup(r => r.Contains("id")).Returns(true);

        // регистрируем мок в IoC
        IoC.Resolve<ICommand>(
            "IoC.Register",
            typeof(IGameObjectRepository).FullName!,
            new Func<object[], object>(_ => repoMock.Object)
        ).Execute();

        var cmd = new ReplaceGameObjectCommand("id", newObj);

        // Act
        cmd.Execute();

        // Assert
        repoMock.Verify(r => r.Replace("id", newObj), Times.Once);
    }

    [Fact]
    public void Execute_Throws_WhenObjectDoesNotExist()
    {
        // Инициализация IoC scope
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
        // Arrange
        var repoMock = new Mock<IGameObjectRepository>();
        repoMock.Setup(r => r.Contains("id")).Returns(false);

        // регистрируем мок в IoC
        IoC.Resolve<ICommand>(
            "IoC.Register",
            typeof(IGameObjectRepository).FullName!,
            new Func<object[], object>(_ => repoMock.Object)
        ).Execute();

        var cmd = new ReplaceGameObjectCommand("id", new GameObject());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }
}
