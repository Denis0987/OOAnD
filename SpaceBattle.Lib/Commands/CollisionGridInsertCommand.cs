namespace SpaceBattle.Lib;

public class CollisionGridInsertCommand
{
    private readonly int _tileSize;
    protected readonly Dictionary<(int, int), HashSet<IMoving>> _tiles = new();

    public CollisionGridInsertCommand(int tileSize)
    {
        _tileSize = tileSize;
    }

    protected (int, int) ComputeTile(Vector location)
    {
        var xPos = location.Coordinates[0];
        var yPos = location.Coordinates[1];
        return ((int)Math.Floor((double)xPos / _tileSize), (int)Math.Floor((double)yPos / _tileSize));
    }

    public void PlaceEntity(IMoving unit)
    {
        var location = unit.Position;
        var tile = ComputeTile(location);
        if (!_tiles.ContainsKey(tile))
        {
            _tiles[tile] = new HashSet<IMoving>();
        }

        _tiles[tile].Add(unit);
    }

    public void ExtractEntity(IMoving unit)
    {
        var location = unit.Position;
        var tile = ComputeTile(location);
        if (_tiles.TryGetValue(tile, out var units))
        {
            units.Remove(unit);
            if (!units.Any())
            {
                _tiles.Remove(tile);
            }
        }
    }
}
