namespace SpaceBattle.Lib;

public class CollisionGridMoveCommand : CollisionGridInsertCommand
{
    public CollisionGridMoveCommand(int tileSize) : base(tileSize) { }

    public void RelocateEntity(IMoving unit, Vector newLocation)
    {
        var currentTile = ComputeTile(unit.Position);
        var targetTile = ComputeTile(newLocation);

        if (currentTile == targetTile)
        {
            unit.Position = newLocation;
            return;
        }

        if (_tiles.TryGetValue(currentTile, out var units))
        {
            units.Remove(unit);
            if (!units.Any())
            {
                _tiles.Remove(currentTile);
            }
        }

        unit.Position = newLocation;
        PlaceEntity(unit);
    }
}