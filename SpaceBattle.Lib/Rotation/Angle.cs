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
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        var other = (Angle)obj;
        return Numerator == other.Numerator && DNumerator == other.DNumerator;
    }

    public static bool operator ==(Angle? angle_1, Angle? angle_2)
    {
        if (ReferenceEquals(angle_1, angle_2))
        {
            return true;
        }

        if (angle_1 is null || angle_2 is null)
        {
            return false;
        }

        return angle_1.Numerator == angle_2.Numerator && angle_1.DNumerator == angle_2.DNumerator;
    }

    public static bool operator !=(Angle? angle_1, Angle? angle_2)
    {
        return !(angle_1 == angle_2);
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
