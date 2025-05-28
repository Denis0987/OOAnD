namespace SpaceBattle.Tests;

public class VectorTests
{

    [Fact]
    public void CreatingVectorWithEmptyCoordinatesTest()
    {
        Assert.Throws<ArgumentException>(() => new Vector());
    }

    [Fact]
    public void CreatingVectorWithNullCoordinates_ThrowsException()
    {
        int[] nullCoords = null!;
        Assert.Throws<ArgumentException>(() => new Vector(nullCoords));
    }

    [Fact]
    public void Equals_WithNonVectorObject_ReturnsFalse()
    {
        // Arrange
        var vector = new Vector(1, 2, 3);
        var notAVector = new object();

        // Act
        var result = vector.Equals(notAVector);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VectorIsNullTest()
    {
        // Arrange
        Vector? v1 = null;
        var v2 = new Vector(2, 4);

        // Act & Assert
        Assert.False(v1 == v2);
        Assert.True(v1 != v2);
    }

    [Fact]
    public void AddingVectorsWithZeroResultTest()
    {
        var v1 = new Vector(1, -1, 2);
        var v2 = new Vector(-1, 1, -2);
        var result = v1 + v2;
        Assert.Equal(new Vector(0, 0, 0), result);
    }

    [Fact]
    public void EqualWithNullTest()
    {
        var v = new Vector(12, 13, 14);
        Assert.False(v.Equals(null));
    }

    [Fact]
    public void VestorsWithDefferentDimansionTest()
    {
        var v1 = new Vector(1, 2, 3);
        var v2 = new Vector(1, 2);
        Assert.Throws<ArgumentException>(() => v1 + v2);
    }

    [Fact]
    public void MethodEqualsEqualVectorsTest()
    {
        var v1 = new Vector(1, 2, 3);
        var v2 = new Vector(1, 2, 3);
        Assert.True(v1.Equals(v2));
    }

    [Fact]
    public void EqualityOperationTest()
    {
        var v1 = new Vector(1, 2, 3);
        var v2 = new Vector(1, 2, 3);
        Assert.True(v1 == v2);
    }

    [Fact]
    public void MethodEqualsUnequalVectorsTest()
    {
        var v1 = new Vector(1, 2, 3);
        var v2 = new Vector(1, 2, 4);
        Assert.False(v1.Equals(v2));
    }

    [Fact]
    public void UnequalityOperationTest()
    {
        var v1 = new Vector(1, 2, 3);
        var v2 = new Vector(1, 2, 4);
        Assert.True(v1 != v2);
    }

    [Fact]
    public void Vector_Has_HashCode_Test()
    {
        var v1 = new Vector(1, 2, 3);
        var v2 = new Vector(1, 2, 3);
        var v3 = new Vector(1, 2, 4);

        Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
        Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var vector = new Vector(1, 2, 3);

        // Act & Assert
        Assert.False(vector.Equals(null));
    }

    [Fact]
    public void AdditionOperator_WithNullVector_ThrowsNullReferenceException()
    {
        // Arrange
        var v1 = new Vector(1, 2, 3);
        Vector? v2 = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => v1 + v2!);
    }
}
