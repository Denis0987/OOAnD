public class Vector
{
    public int[] Coordinates { get; }
    public Vector(params int[] coordinates)
    {
        if (coordinates == null || coordinates.Length == 0)
        {
            throw new ArgumentException("Ошибка! Вектор не может быть пустым ");
        }

        Coordinates = coordinates;
    }
    public static Vector operator +(Vector v1, Vector v2)
    {
        if (v1.Coordinates.Length != v2.Coordinates.Length)
        {
            throw new ArgumentException("Ошибка! Вектора не могут иметь разные размерности! ");
        }

        var resultCoordinates = v1.Coordinates.Zip(v2.Coordinates, (x, y) => x + y).ToArray();
        return new Vector(resultCoordinates);
    }

    public override bool Equals(object? obj)
    {
        return (obj is Vector otherVector && obj != null && Coordinates.SequenceEqual(otherVector.Coordinates));
    }
    public override int GetHashCode()
    {
        return Coordinates.Aggregate(HashCode.Combine(Coordinates.Length), (current, coordinate) =>
            HashCode.Combine(current, coordinate));
    }

    public static bool operator ==(Vector? v1, Vector? v2)
    {
        return !(v1 is null || v2 is null) && v1.Equals(v2);
    }

    public static bool operator !=(Vector? v1, Vector? v2)
    {
        return !(v1 == v2);
    }
}
