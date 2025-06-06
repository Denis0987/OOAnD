namespace SpaceBattle.Lib;

public class RegisterIoCDependencyCollisionVerifyCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.IsColliding",
            (object[] entities) =>
            {
                var (deltas, nodeType) = IoC.Resolve<(int[], string)>(
                    "Collision.CalculateDeltas", entities[0], entities[1]);

                object node = IoC.Resolve<IDictionary<int, object>>($"Collision.Nodes.{nodeType}");

                return (object)deltas.All(delta =>
                    node is IDictionary<int, object> nodeDict &&
                    nodeDict.TryGetValue(delta, out node!));
            }
        ).Execute();
    }
}
