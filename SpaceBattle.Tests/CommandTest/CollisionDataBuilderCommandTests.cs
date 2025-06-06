using Hwdtech.Ioc;

namespace SpaceBattle.Tests;

public class CollisionDataBuilderCommandTests
{
    public CollisionDataBuilderCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void BuildCollisionData_ProducesCorrectSequence()
    {
        var mockFormRecognizer = new Mock<IShapeRecognizer>();

        mockFormRecognizer.Setup(s => s.GetFormId("rectangle")).Returns("rectangle".GetHashCode());
        mockFormRecognizer.Setup(s => s.GetFormId("pentagon")).Returns("pentagon".GetHashCode());

        var locationA = new Vector(15, 25);
        var locationB = new Vector(10, 20);
        var speedA = new Vector(2, 3);
        var speedB = new Vector(4, 5);

        var builder = new CollisionDataBuilderCommand(mockFormRecognizer.Object);
        var data = builder.BuildCollisionData(
            locationA, speedA, "rectangle",
            locationB, speedB, "pentagon").ToArray();

        Assert.NotNull(data);
        Assert.Equal(10, data.Length);

        Assert.Equal(15, data[0]);
        Assert.Equal(25, data[1]);
        Assert.Equal(10, data[2]);
        Assert.Equal(20, data[3]);

        Assert.Equal(2, data[4]);
        Assert.Equal(3, data[5]);
        Assert.Equal(4, data[6]);
        Assert.Equal(5, data[7]);

        Assert.Equal("rectangle".GetHashCode(), data[8]);
        Assert.Equal("pentagon".GetHashCode(), data[9]);
    }

    [Fact]
    public void BuildCollisionData_WithVariedForms_ReturnsCorrectFormIds()
    {
        var mockFormRecognizer = new Mock<IShapeRecognizer>();

        mockFormRecognizer.Setup(s => s.GetFormId("rectangle")).Returns("rectangle".GetHashCode());
        mockFormRecognizer.Setup(s => s.GetFormId("pentagon")).Returns("pentagon".GetHashCode());
        mockFormRecognizer.Setup(s => s.GetFormId("hexagon")).Returns("hexagon".GetHashCode());
        mockFormRecognizer.Setup(s => s.GetFormId("octagon")).Returns("octagon".GetHashCode());

        var location = new Vector(0, 0);
        var speed = new Vector(0, 0);

        var builder = new CollisionDataBuilderCommand(mockFormRecognizer.Object);

        var data1 = builder.BuildCollisionData(
            location, speed, "rectangle",
            location, speed, "pentagon").ToArray();

        Assert.Equal("rectangle".GetHashCode(), data1[8]);
        Assert.Equal("pentagon".GetHashCode(), data1[9]);

        var data2 = builder.BuildCollisionData(
            location, speed, "hexagon",
            location, speed, "octagon").ToArray();

        Assert.Equal("hexagon".GetHashCode(), data2[8]);
        Assert.Equal("octagon".GetHashCode(), data2[9]);
    }
}
