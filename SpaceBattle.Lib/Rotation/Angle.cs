namespace SpaceBattle.Lib;

public class Angle
{
    private int Numerator { get; set; }
    private int DNumerator { get; }

    public Angle(int Numerator, int DNumerator)
    {
        this.Numerator = ((Numerator % DNumerator) + DNumerator) % DNumerator;
        this.DNumerator = DNumerator;
    }

    public static Angle operator +(Angle angle_1, Angle angle_2)
    {
        return new Angle(angle_1.Numerator + angle_2.Numerator, angle_1.DNumerator);
    }

    public override bool Equals(object? obj)
    {
        return obj != null && obj is Angle angle && angle.Numerator == Numerator && angle.DNumerator == DNumerator;
    }

    public static bool operator ==(Angle angle_1, Angle angle_2)
    {
        return angle_1.Equals(angle_2);
    }

    public static bool operator !=(Angle angle_1, Angle angle_2)
    {
        return !(angle_1.Equals(angle_2));
    }

    public override int GetHashCode()
    {
        return Numerator.GetHashCode();
    }

    public static implicit operator double(Angle angle)
    {
        return 2 * Math.PI * angle.Numerator / angle.DNumerator;
    }
}
