using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class CollisionGridInsertRemoveTests
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
    public void AddSingleEntityToGrid()
    {
        var entity = new MockMoving(20, 30);
        var collisionGrid = new CollisionGridInsertCommand(10);
        
        // Test that placing an entity doesn't throw
        var exception = Record.Exception(() => collisionGrid.PlaceEntity(entity));
        Assert.Null(exception);
    }

    [Fact]
    public void AddMultipleEntitiesToGrid()
    {
        var entity1 = new MockMoving(20, 30);
        var entity2 = new MockMoving(21, 31);
        var collisionGrid = new CollisionGridInsertCommand(10);

        // Test that placing multiple entities doesn't throw
        var exception1 = Record.Exception(() => collisionGrid.PlaceEntity(entity1));
        var exception2 = Record.Exception(() => collisionGrid.PlaceEntity(entity2));
        
        Assert.Null(exception1);
        Assert.Null(exception2);
    }

    [Fact]
    public void RemoveExistingEntityFromGrid()
    {
        var entity = new MockMoving(20, 30);
        var collisionGrid = new CollisionGridInsertCommand(10);
        collisionGrid.PlaceEntity(entity);

        // Test that removing an existing entity doesn't throw
        var exception = Record.Exception(() => collisionGrid.ExtractEntity(entity));
        Assert.Null(exception);
    }

    [Fact]
    public void RemoveNonExistingEntityFromGrid()
    {
        var entity = new MockMoving(20, 30);
        var collisionGrid = new CollisionGridInsertCommand(10);

        // Test that removing a non-existing entity doesn't throw
        var exception = Record.Exception(() => collisionGrid.ExtractEntity(entity));
        Assert.Null(exception);
    }
}