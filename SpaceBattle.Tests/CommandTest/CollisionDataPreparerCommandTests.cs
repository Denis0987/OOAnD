using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class PrepareCollisionDataCommandTests
{
    public PrepareCollisionDataCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void PrepareCollisionDataCommand_ProcessesDataCorrectly()
    {
        var testData = new List<int[]> { new[] { 10, 20, 30 }, new[] { 40, 50, 60 } };
        var mockSource = new Mock<ICollisionDataProvider>();

        mockSource.Setup(s => s.FirstId).Returns("entity1");
        mockSource.Setup(s => s.SecondId).Returns("entity2");
        mockSource.Setup(s => s.GetCollisionData()).Returns(testData);

        var mockStoreCommand = new Mock<ICommand>();

        IoC.Resolve<ICommand>("IoC.Register", "Collision.GenerateFileName",
            (object[] inputs) => $"{inputs[0]}-{inputs[1]}.txt").Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Collision.StoreData",
            (object[] inputs) =>
            {
                Assert.Equal("entity1-entity2.txt", inputs[0]);
                Assert.Equal(testData, inputs[1]);
                return mockStoreCommand.Object;
            }).Execute();

        var command = new PrepareCollisionDataCommand(mockSource.Object);
        command.Execute();

        mockSource.Verify(s => s.FirstId, Times.Once);
        mockSource.Verify(s => s.SecondId, Times.Once);
        mockSource.Verify(s => s.GetCollisionData(), Times.Once);
        mockStoreCommand.Verify(c => c.Execute(), Times.Once);
    }
}
