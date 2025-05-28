namespace SpaceBattle.Tests;
using SpaceBattle.Lib;

public class AngleTest
{
    [Fact]
    public void AngleSum()
    {
        var angle_1 = new Angle(5, 8);
        var angle_2 = new Angle(7, 8);
        Assert.Equal(new Angle(4, 8), angle_1 + angle_2);
    }

    [Fact]
    public void EqualsTest_True()
    {
        var angle_1 = new Angle(15, 8);
        var angle_2 = new Angle(23, 8);
        Assert.True(angle_1.Equals(angle_2));
    }

    [Fact]
    public void OperatorEqualsTest_True()
    {
        var angle_1 = new Angle(15, 8);
        var angle_2 = new Angle(23, 8);
        Assert.True(angle_1 == angle_2);
    }

    [Fact]
    public void EqualsTest_False()
    {
        var angle_1 = new Angle(1, 8);
        var angle_2 = new Angle(2, 8);
        Assert.False(angle_1.Equals(angle_2));
    }

    [Fact]
    public void OperatorNotEqualsTest_True()
    {
        var angle_1 = new Angle(1, 8);
        var angle_2 = new Angle(2, 8);
        Assert.True(angle_1 != angle_2);
    }

    [Fact]
    public void GetHashCodeTest()
    {
        var angle_1 = new Angle(6, 8);
        var angle_2 = new Angle(6, 8);
        var hashcode = angle_1.GetHashCode();
        Assert.Equal(hashcode, angle_2.GetHashCode());
    }

    [Fact]
    public void SinTest()
    {
        var angle = new Angle(2, 8);
        Assert.Equal(1.0, Math.Round(Math.Sin(angle), 3));
    }

    [Fact]
    public void CosTest()
    {
        var angle = new Angle(0, 8);
        Assert.Equal(1.0, Math.Round(Math.Cos(angle), 3));
    }

    [Fact]
    public void Constructor_WithNegativeNumerator_NormalizesAngle()
    {
        // Arrange & Act
        var angle = new Angle(-1, 8);
        var expected = new Angle(7, 8);

        // Assert - Test the behavior through the public API
        Assert.Equal(expected, angle);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var angle = new Angle(1, 8);

        // Act & Assert
        Assert.False(angle.Equals(null));
    }

    [Fact]
    public void OperatorEquals_WithFirstNull_ReturnsFalse()
    {
        // Arrange
        Angle? angle1 = null;
        var angle2 = new Angle(1, 8);

        // Act & Assert
        Assert.False(angle1 == angle2);
    }

    [Fact]
    public void OperatorNotEquals_WithFirstNull_ReturnsTrue()
    {
        // Arrange
        Angle? angle1 = null;
        var angle2 = new Angle(1, 8);

        // Act & Assert
        Assert.True(angle1 != angle2);
    }

    [Fact]
    public void ImplicitConversionToDouble_ReturnsCorrectValue()
    {
        // Arrange
        var angle = new Angle(4, 8); // 4/8 = 0.5 of full circle

        // Act
        double result = angle;

        // Assert
        Assert.Equal(Math.PI, result, 5); // 0.5 * 2π = π
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var angle = new Angle(1, 8);
        var notAnAngle = new object();

        // Act & Assert
        Assert.False(angle.Equals(notAnAngle));
    }

    [Fact]
    public void Equals_WithDifferentTypeInheritedFromAngle_ReturnsFalse()
    {
        // Arrange
        var angle = new Angle(1, 8);
        var derivedAngle = new DerivedAngle(2, 8);

        // Act & Assert
        Assert.False(angle.Equals(derivedAngle));
    }

    private class DerivedAngle : Angle
    {
        public DerivedAngle(int numerator, int dNumerator) : base(numerator, dNumerator) { }
    }

    [Fact]
    public void OperatorEquals_BothNull_ReturnsTrue()
    {
        // Arrange
        Angle? angle1 = null;
        Angle? angle2 = null;

        // Act & Assert
        Assert.True(angle1 == angle2);
    }

    [Fact]
    public void OperatorEquals_WithSecondNull_ReturnsFalse()
    {
        // Arrange
        var angle1 = new Angle(1, 8);
        Angle? angle2 = null;

        // Act & Assert
        Assert.False(angle1 == angle2);
    }

    [Fact]
    public void OperatorNotEquals_WithSecondNull_ReturnsTrue()
    {
        // Arrange
        var angle1 = new Angle(1, 8);
        Angle? angle2 = null;

        // Act & Assert
        Assert.True(angle1 != angle2);
    }

    [Fact]
    public void Equals_WithDerivedType_ReturnsFalse()
    {
        // Arrange
        var angle = new Angle(1, 8);
        var derived = new DerivedAngle(1, 8);

        // Act & Assert
        Assert.False(angle.Equals(derived));
        Assert.False(derived.Equals(angle));
    }
}
