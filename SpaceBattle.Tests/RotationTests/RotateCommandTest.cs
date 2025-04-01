using SpaceBattle.Lib;
namespace SpaceBattle.Tests;

public class RotateCommandTest
{
    [Fact]
    public void Rotating45_Velocity90Test()
    {
        var rotatingObj = new Mock<IRotating>();
        rotatingObj.SetupGet(x => x.AnglePosition).Returns(new Angle(45, 8));
        rotatingObj.SetupGet(x => x.RotateVelocity).Returns(new Angle(45, 8));
        var rotateCommand = new RotateCommand(rotatingObj.Object);
        rotateCommand.Execute();
        rotatingObj.VerifySet(x => x.AnglePosition = new Angle(90, 8));
    }

    [Fact]
    public void NoAnglePositionTest()
    {
        var rotatingObj = new Mock<IRotating>();
        rotatingObj.SetupGet(x => x.AnglePosition).Throws<Exception>();
        rotatingObj.SetupGet(x => x.RotateVelocity).Returns(new Angle(90, 8));
        var rotateCommand = new RotateCommand(rotatingObj.Object);
        Assert.Throws<Exception>(() => rotateCommand.Execute());
    }

    [Fact]
    public void NoRotateVelocityTest()
    {
        var rotatingObj = new Mock<IRotating>();
        rotatingObj.SetupGet(x => x.AnglePosition).Returns(new Angle(45, 8));
        rotatingObj.SetupGet(x => x.RotateVelocity).Throws<Exception>();
        var rotateCommand = new RotateCommand(rotatingObj.Object);
        Assert.Throws<Exception>(() => rotateCommand.Execute());
    }

    [Fact]
    public void CantChangeAnglePosTest()
    {
        var rotatingObj = new Mock<IRotating>();
        rotatingObj.SetupGet(x => x.AnglePosition).Returns(new Angle(0, 8));
        rotatingObj.SetupSet(x => x.AnglePosition = It.IsAny<Angle>()).Throws<Exception>();
        rotatingObj.SetupGet(x => x.RotateVelocity).Returns(new Angle(657, 8));
        var rotateCommand = new RotateCommand(rotatingObj.Object);
        Assert.Throws<Exception>(() => rotateCommand.Execute());
    }
}
