namespace SpaceBattle.Lib;

public class RegisterIoCDependencyCollisionCalcCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.CalculateDeltas",
            (object[] entities) =>
            {
                var firstEntity = entities[0];
                var secondEntity = entities[1];

                var typeA = IoC.Resolve<string>("Entity.Type", firstEntity);
                var typeB = IoC.Resolve<string>("Entity.Type", secondEntity);
                var typePairs = IoC.Resolve<IDictionary<(string, string), string>>("Collision.TypePairs");

                var primaryType = typePairs.TryGetValue((typeA, typeB), out var t) ? t :
                                  typePairs.TryGetValue((typeB, typeA), out t) ? t : typeA;

                var isFirstPrimary = primaryType == typeA;
                var primaryEntity = isFirstPrimary ? firstEntity : secondEntity;
                var secondaryEntity = isFirstPrimary ? secondEntity : firstEntity;

                var deltas = IoC.Resolve<int[]>("Entity.Position", primaryEntity)
                    .Zip(IoC.Resolve<int[]>("Entity.Position", secondaryEntity), (p, s) => p - s)
                    .Concat(IoC.Resolve<int[]>("Entity.Velocity", primaryEntity)
                    .Zip(IoC.Resolve<int[]>("Entity.Velocity", secondaryEntity), (p, s) => p - s))
                    .ToArray();

                return (object)(deltas, $"{primaryType}{(isFirstPrimary ? typeB : typeA)}");
            }
        ).Execute();
    }
}
