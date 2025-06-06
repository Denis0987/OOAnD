using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class CollisionGridMoveTests
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
    public void MoveEntityWithinSameTile()
    {
        var entity = new MockMoving(20, 30);
        var collisionGrid = new CollisionGridMoveCommand(10);

        // Test that placing and relocating an entity doesn't throw
        var placeException = Record.Exception(() => collisionGrid.PlaceEntity(entity));
        var moveException = Record.Exception(() => collisionGrid.RelocateEntity(entity, new Vector(21, 31)));

        Assert.Null(placeException);
        Assert.Null(moveException);
    }

    [Fact]
    public void MoveEntityToAnotherTile()
    {
        var entity = new MockMoving(20, 30);
        var collisionGrid = new CollisionGridMoveCommand(10);

        // Test that placing and relocating an entity to a different tile doesn't throw
        var placeException = Record.Exception(() => collisionGrid.PlaceEntity(entity));
        var moveException = Record.Exception(() => collisionGrid.RelocateEntity(entity, new Vector(40, 50)));

        Assert.Null(placeException);
        Assert.Null(moveException);
    }
}
