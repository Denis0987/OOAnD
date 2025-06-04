namespace SpaceBattle.Lib.Tests.CommandTests;

using System.Collections.Generic;
using Moq;
using SpaceBattle.Lib.Commands;
using SpaceBattle.Lib.Interfaces;
using Xunit;

public class CollisionDataPreparerCommandTests
{
    public CollisionDataPreparerCommandTests()
    {
        try
        {
            // Get the root scope
            var rootScope = IoC.Resolve<object>("Scopes.Root");

            // Create a new scope for the test
            var scope = IoC.Resolve<object>("Scopes.New", rootScope);
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            // Register required dependencies
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "IoC.Scope.Current",
                (object[] _) => scope
            ).Execute();

            // Register storage directory
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => "./collisions"
            ).Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in test setup: {ex}");
            throw;
        }
    }

    [Fact]
    public void Execute_ShouldGatherDataAndCallSave()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 7, 8, 9 }, new[] { 10, 11, 12 } };
        var mockProvider = new Mock<ICollisionInfoProvider>();

        mockProvider.Setup(p => p.FirstObjectId).Returns("Alpha");
        mockProvider.Setup(p => p.SecondObjectId).Returns("Beta");
        mockProvider.Setup(p => p.GetCollisionPoints()).Returns(samplePoints);

        // Регистрируем стратегию форматирования имени
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.FileNameFormatter",
            (object[] args) =>
            {
                Assert.Equal("Alpha", args[0]);
                Assert.Equal("Beta", args[1]);
                return "Alpha__Beta.log";
            }
        ).Execute();

        // Подготавливаем мок команды сохранения, проверяя дальнейшие аргументы
        var saveMock = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.DataSaver",
            (object[] args) =>
            {
                // Проверим, что сюда придут ожидаемые значения
                Assert.Equal("Alpha__Beta.log", args[0]);

                // Сравним списки через ReferenceEquals или SequenceEqual
                var passedList = args[1] as IList<int[]>;
                Assert.Equal(samplePoints, passedList);

                return saveMock.Object;
            }
        ).Execute();

        // Act
        var preparerCmd = new CollisionDataPreparerCommand(mockProvider.Object);
        preparerCmd.Execute();

        // Assert: 
        mockProvider.Verify(p => p.FirstObjectId, Times.Once);
        mockProvider.Verify(p => p.SecondObjectId, Times.Once);
        mockProvider.Verify(p => p.GetCollisionPoints(), Times.Once);
        saveMock.Verify(c => c.Execute(), Times.Once);
    }
}
