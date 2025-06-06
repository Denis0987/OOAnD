using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class CollisionGridQueryTests
{
    private class MockMoving : IMoving
    {
        public Vector Position { get; set; }
        public Vector Velocity { get; } = new Vector(0, 0);
        private readonly Guid _identifier = Guid.NewGuid();

        public MockMoving(params int[] coordinates)
        {
            Position = new Vector(coordinates);
        }

        public override bool Equals(object? other)
        {
            return other is MockMoving m && _identifier.Equals(m._identifier);
        }

        public override int GetHashCode()
        {
            return _identifier.GetHashCode();
        }
    }

    [Fact]
    public void FindNearbyEntities_SameTile()
    {
        var entity1 = new MockMoving(20, 30);
        var entity2 = new MockMoving(21, 31);
        var collisionGrid = new CollisionGridQueryCommand(10);
        collisionGrid.PlaceEntity(entity1);
        collisionGrid.PlaceEntity(entity2);

        var nearbyUnits = collisionGrid.LocateNearbyUnits(entity1);

        Assert.Contains(entity2, nearbyUnits);
        Assert.DoesNotContain(entity1, nearbyUnits);
    }

    [Fact]
    public void FindNearbyEntities_AdjacentTile()
    {
        var entity1 = new MockMoving(20, 30);
        var entity2 = new MockMoving(30, 30);
        var collisionGrid = new CollisionGridQueryCommand(10);
        collisionGrid.PlaceEntity(entity1);
        collisionGrid.PlaceEntity(entity2);

        var nearbyUnits = collisionGrid.LocateNearbyUnits(entity1);
        Assert.Contains(entity2, nearbyUnits);
        Assert.DoesNotContain(entity1, nearbyUnits);
    }

    [Fact]
    public void FindNearbyEntities_EmptyGrid()
    {
        var entity = new MockMoving(20, 30);
        var collisionGrid = new CollisionGridQueryCommand(10);

        var nearbyUnits = collisionGrid.LocateNearbyUnits(entity);
        Assert.Empty(nearbyUnits);
    }

    [Fact]
    public void GetEntitiesInExistingTile()
    {
        var entity = new MockMoving(20, 30);
        var collisionGrid = new CollisionGridQueryCommand(10);
        collisionGrid.PlaceEntity(entity);

        var tile = (2, 3);
        var entities = collisionGrid.GetUnitsInTile(tile);

        Assert.Contains(entity, entities);
    }

    [Fact]
    public void GetEntitiesInNonExistingTile()
    {
        var collisionGrid = new CollisionGridQueryCommand(10);
        var tile = (2, 3);

        var entities = collisionGrid.GetUnitsInTile(tile);
        Assert.Empty(entities);
    }

    [Fact]
    public void ListAllActiveTiles()
    {
        var entity1 = new MockMoving(20, 30);
        var entity2 = new MockMoving(40, 50);
        var collisionGrid = new CollisionGridQueryCommand(10);

        collisionGrid.PlaceEntity(entity1);
        collisionGrid.PlaceEntity(entity2);

        var activeTiles = collisionGrid.ListActiveTiles().ToList();
        Assert.Contains((2, 3), activeTiles);
        Assert.Contains((4, 5), activeTiles);
        Assert.Equal(2, activeTiles.Count);
    }
}
