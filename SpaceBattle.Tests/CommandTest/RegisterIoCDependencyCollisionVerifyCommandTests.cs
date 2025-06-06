using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class RegisterIoCDependencyCollisionVerifyCommandTests
{
    private readonly int[] _deltas;
    private readonly string _nodeType;

    public RegisterIoCDependencyCollisionVerifyCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        _deltas = new[] { 7, 8, 9 };
        _nodeType = "CollisionNode";
    }

    [Fact]
    public void VerifyCollision_ReturnsTrue_WhenPathExists()
    {
        var collisionNodes = new Dictionary<int, object>
        {
            [7] = new Dictionary<int, object>
            {
                [8] = new Dictionary<int, object>
                {
                    [9] = new object()
                }
            }
        };

        IoC.Resolve<ICommand>("IoC.Register", "Collision.CalculateDeltas",
            (object[] entities) => (object)(_deltas, _nodeType)).Execute();

        IoC.Resolve<ICommand>("IoC.Register", $"Collision.Nodes.{_nodeType}",
            (object[] entities) => collisionNodes).Execute();

        new RegisterIoCDependencyCollisionVerifyCommand().Execute();

        Assert.True(IoC.Resolve<bool>("Collision.IsColliding", new object(), new object()));
    }

    [Fact]
    public void VerifyCollision_ReturnsFalse_WhenPathMissing()
    {
        var collisionNodes = new Dictionary<int, object>
        {
            [7] = new Dictionary<int, object>()
        };

        IoC.Resolve<ICommand>("IoC.Register", "Collision.CalculateDeltas",
            (object[] entities) => (object)(_deltas, _nodeType)).Execute();

        IoC.Resolve<ICommand>("IoC.Register", $"Collision.Nodes.{_nodeType}",
            (object[] entities) => collisionNodes).Execute();

        new RegisterIoCDependencyCollisionVerifyCommand().Execute();

        Assert.False(IoC.Resolve<bool>("Collision.IsColliding", new object(), new object()));
    }

    [Fact]
    public void VerifyCollision_ReturnsFalse_WhenFirstDeltaMissing()
    {
        var collisionNodes = new Dictionary<int, object>();

        IoC.Resolve<ICommand>("IoC.Register", "Collision.CalculateDeltas",
            (object[] entities) => (object)(_deltas, _nodeType)).Execute();

        IoC.Resolve<ICommand>("IoC.Register", $"Collision.Nodes.{_nodeType}",
            (object[] entities) => collisionNodes).Execute();

        new RegisterIoCDependencyCollisionVerifyCommand().Execute();

        Assert.False(IoC.Resolve<bool>("Collision.IsColliding", new object(), new object()));
    }

    [Fact]
    public void VerifyCollision_ReturnsFalse_WhenNodeNotDictionary()
    {
        var collisionNodes = new Dictionary<int, object>
        {
            [7] = new Dictionary<int, object>
            {
                [8] = new object()
            }
        };

        IoC.Resolve<ICommand>("IoC.Register", "Collision.CalculateDeltas",
            (object[] entities) => (object)(_deltas, _nodeType)).Execute();

        IoC.Resolve<ICommand>("IoC.Register", $"Collision.Nodes.{_nodeType}",
            (object[] entities) => collisionNodes).Execute();

        new RegisterIoCDependencyCollisionVerifyCommand().Execute();

        Assert.False(IoC.Resolve<bool>("Collision.IsColliding", new object(), new object()));
    }
}
