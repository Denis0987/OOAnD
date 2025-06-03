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

    [Fact]
    public void Rotate_WithAngleNormalization_WrapsAroundCorrectly()
    {
        // Arrange
        var rotatingObj = new Mock<IRotating>();
        rotatingObj.SetupGet(x => x.AnglePosition).Returns(new Angle(7, 8));  // 315 degrees
        rotatingObj.SetupGet(x => x.RotateVelocity).Returns(new Angle(1, 8));  // 45 degrees
        var rotateCommand = new RotateCommand(rotatingObj.Object);

        // Act
        rotateCommand.Execute();

        // Assert - Should wrap around to 0/8 (0 degrees)
        rotatingObj.VerifySet(x => x.AnglePosition = new Angle(0, 8));
    }

    [Fact]
    public void Rotate_WithNegativeAnglePosition_HandlesCorrectly()
    {
        // Arrange - This tests the Angle constructor's handling of negative values
        var rotatingObj = new Mock<IRotating>();
        rotatingObj.SetupGet(x => x.AnglePosition).Returns(new Angle(-1, 8));  // Should normalize to 7/8 (315 degrees)
        rotatingObj.SetupGet(x => x.RotateVelocity).Returns(new Angle(1, 8));   // 45 degrees
        var rotateCommand = new RotateCommand(rotatingObj.Object);

        // Act
        rotateCommand.Execute();

        // Assert - -1/8 + 1/8 = 0/8 (0 degrees)
        rotatingObj.VerifySet(x => x.AnglePosition = new Angle(0, 8));
    }

    [Fact]
    public void Rotate_WithZeroVelocity_DoesNotChangeAngle()
    {
        // Arrange
        var rotatingObj = new Mock<IRotating>();
        var initialAngle = new Angle(3, 8);  // 135 degrees
        rotatingObj.SetupGet(x => x.AnglePosition).Returns(initialAngle);
        rotatingObj.SetupGet(x => x.RotateVelocity).Returns(new Angle(0, 8));  // 0 degrees
        var rotateCommand = new RotateCommand(rotatingObj.Object);

        // Act
        rotateCommand.Execute();

        // Assert - Angle should remain unchanged
        rotatingObj.VerifySet(x => x.AnglePosition = initialAngle);
    }

    [Fact]
    public void Rotate_WithLargeVelocity_HandlesMultipleRotations()
    {
        // Arrange
        var rotatingObj = new Mock<IRotating>();
        rotatingObj.SetupGet(x => x.AnglePosition).Returns(new Angle(0, 8));
        // 2 full rotations + 90 degrees
        rotatingObj.SetupGet(x => x.RotateVelocity).Returns(new Angle(16 + 2, 8));
        var rotateCommand = new RotateCommand(rotatingObj.Object);

        // Act
        rotateCommand.Execute();

        // Assert - Should be at 90 degrees (2/8)
        rotatingObj.VerifySet(x => x.AnglePosition = new Angle(2, 8));
    }

    [Fact]
    public void Rotate_WithMaxAngleValue_HandlesCorrectly()
    {
        // Arrange
        var rotatingObj = new Mock<IRotating>();
        rotatingObj.SetupGet(x => x.AnglePosition).Returns(new Angle(7, 8));  // 315 degrees
        rotatingObj.SetupGet(x => x.RotateVelocity).Returns(new Angle(1, 8));  // 45 degrees
        var rotateCommand = new RotateCommand(rotatingObj.Object);

        // Act
        rotateCommand.Execute();

        // Assert - Should wrap around to 0/8 (0 degrees)
        rotatingObj.VerifySet(x => x.AnglePosition = new Angle(0, 8));
    }
}
