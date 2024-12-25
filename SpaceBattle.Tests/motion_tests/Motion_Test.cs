
using SpaceBattle.Lib;
namespace SpaceBattle.Tests
{
    public class MoveCommandTests
    {
        [Fact]
        public void MoveCommand_MovesObjectToCorrectPosition()
        {
            var mockObject = new Mock<IMoving>();
            mockObject.SetupGet(m => m.Position).Returns(new Vector(12, 5));
            mockObject.SetupGet(m => m.Velocity).Returns(new Vector(-4, 1));
            var moveCommand = new MoveCommand(mockObject.Object);
            moveCommand.Execute();
            mockObject.VerifySet(m => m.Position = new Vector(8, 6));
        }

        [Fact]
        public void MoveCommand_ThrowsExceptionWhenPositionIsUndefined()
        {
            var mockObject = new Mock<IMoving>();
            mockObject.SetupGet(m => m.Position).Throws<Exception>();
            var moveCommand = new MoveCommand(mockObject.Object);
            Assert.Throws<Exception>(() => moveCommand.Execute());
        }

        [Fact]
        public void MoveCommand_ThrowsExceptionWhenVelocityIsUndefined()
        {
            var mockObject = new Mock<IMoving>();
            mockObject.SetupGet(m => m.Velocity).Throws<Exception>();
            var moveCommand = new MoveCommand(mockObject.Object);
            Assert.Throws<Exception>(() => moveCommand.Execute());
        }

        [Fact]
        public void MoveCommand_ThrowsExceptionWhenPositionCannotBeChanged()
        {

            var mockObject = new Mock<IMoving>();
            mockObject.SetupGet(m => m.Position).Returns(new Vector(12, 5));
            mockObject.SetupGet(m => m.Velocity).Returns(new Vector(-4, 1));
            mockObject.SetupSet(m => m.Position = It.IsAny<Vector>()).Throws<Exception>();

            var moveCommand = new MoveCommand(mockObject.Object);

            Assert.Throws<Exception>(() => moveCommand.Execute());
        }
    }
} 
