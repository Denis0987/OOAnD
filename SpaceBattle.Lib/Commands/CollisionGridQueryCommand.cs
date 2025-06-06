namespace SpaceBattle.Lib;

public class CollisionGridQueryCommand : CollisionGridInsertCommand
{
    public CollisionGridQueryCommand(int tileSize) : base(tileSize) { }

    public IEnumerable<IMoving> LocateNearbyUnits(IMoving unit)
    {
        var location = unit.Position;
        var (tileX, tileY) = ComputeTile(location);

        var adjacentTiles = from shiftX in Enumerable.Range(-1, 3)
                            from shiftY in Enumerable.Range(-1, 3)
                            select (tileX + shiftX, tileY + shiftY);

        var nearbyUnits = adjacentTiles
            .Where(tile => _tiles.ContainsKey(tile))
            .SelectMany(tile => _tiles[tile])
            .Where(u => u != unit);

        return nearbyUnits;
    }

    public IEnumerable<(int, int)> ListActiveTiles() => _tiles.Keys;

    public IEnumerable<IMoving> GetUnitsInTile((int, int) tile) =>
        _tiles.TryGetValue(tile, out var units) ? units : Enumerable.Empty<IMoving>();
}