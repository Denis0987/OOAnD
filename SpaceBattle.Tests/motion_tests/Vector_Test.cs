namespace SpaceBattle.Tests;

public class VectorTests
{

    [Fact]
    public void CreatingVectorWithEmptyCoordinatesTest()
    {
        Assert.Throws<ArgumentException>(() => new Vector());
    }

    [Fact]
    public void VectorIsNullTest()
    {
        Vector? v1 = null;
        var v2 = new Vector(2, 4);
        Assert.False(v1 == v2);
        Assert.True(v1 != v2);
    }

    [Fact]
    public void BothVectorsAreNullTest()
    {
        Vector v1 = null;
        Vector v2 = null;
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
}
