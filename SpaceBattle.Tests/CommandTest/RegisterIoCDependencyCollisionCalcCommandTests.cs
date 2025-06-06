using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class RegisterIoCDependencyCollisionCalcCommandTests
{
    private readonly object _firstEntity, _secondEntity;
    private readonly string _typeA, _typeB;
    private readonly int[] _positionA, _positionB;
    private readonly int[] _velocityA, _velocityB;

    public RegisterIoCDependencyCollisionCalcCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        _firstEntity = new object();
        _secondEntity = new object();
        _typeA = "EntityTypeA";
        _typeB = "EntityTypeB";
        _positionA = new[] { 20, 30 };
        _positionB = new[] { 15, 20 };
        _velocityA = new[] { 7, 8 };
        _velocityB = new[] { 3, 3 };
    }

    [Fact]
    public void CalculateDeltas_ReturnsCorrectDeltas_AndNodeType()
    {
        var typePairs = new Dictionary<(string, string), string>
        {
            { (_typeA, _typeB), _typeA }
        };

        RegisterDependencies(typePairs);

        var result = IoC.Resolve<(int[], string)>("Collision.CalculateDeltas", _firstEntity, _secondEntity);

        Assert.Equal(5, result.Item1[0]);
        Assert.Equal(10, result.Item1[1]);
        Assert.Equal(4, result.Item1[2]);
        Assert.Equal(5, result.Item1[3]);
        Assert.Equal($"{_typeA}{_typeB}", result.Item2);
    }

    [Fact]
    public void CalculateDeltas_HandlesReversePair()
    {
        var typePairs = new Dictionary<(string, string), string>
        {
            { (_typeB, _typeA), _typeB }
        };

        RegisterDependencies(typePairs);

        var result = IoC.Resolve<(int[], string)>("Collision.CalculateDeltas", _firstEntity, _secondEntity);

        Assert.Equal(-5, result.Item1[0]);
        Assert.Equal(-10, result.Item1[1]);
        Assert.Equal(-4, result.Item1[2]);
        Assert.Equal(-5, result.Item1[3]);
        Assert.Equal($"{_typeB}{_typeA}", result.Item2);
    }

    [Fact]
    public void CalculateDeltas_UsesDefaultType_WhenPairNotDefined()
    {
        RegisterDependencies(new Dictionary<(string, string), string>());

        var result = IoC.Resolve<(int[], string)>("Collision.CalculateDeltas", _firstEntity, _secondEntity);

        Assert.Equal($"{_typeA}{_typeB}", result.Item2);
    }

    private void RegisterDependencies(Dictionary<(string, string), string> typePairs)
    {
        IoC.Resolve<ICommand>("IoC.Register", "Entity.Type",
            (object[] entities) => entities[0] == _firstEntity ? _typeA : _typeB).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Collision.TypePairs",
            (object[] entities) => typePairs).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Entity.Position",
            (object[] entities) => entities[0] == _firstEntity ? _positionA : _positionB).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Entity.Velocity",
            (object[] entities) => entities[0] == _firstEntity ? _velocityA : _velocityB).Execute();

        new RegisterIoCDependencyCollisionCalcCommand().Execute();
    }
}
