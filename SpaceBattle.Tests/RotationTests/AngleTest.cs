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
}
