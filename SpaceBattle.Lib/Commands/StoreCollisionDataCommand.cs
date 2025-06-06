namespace SpaceBattle.Lib;

public class StoreCollisionDataCommand : ICommand
{
    private readonly string _targetFile;
    private readonly IList<int[]> _impactData;

    public StoreCollisionDataCommand(string targetFile, IList<int[]> impactData)
    {
        _targetFile = targetFile;
        _impactData = impactData;
    }

    public void Execute()
    {
        var storagePath = IoC.Resolve<string>("Collision.StoragePath");
        var fullPath = Path.Combine(storagePath, _targetFile);
        var textLines = _impactData.Select(vector => string.Join(" ", vector)).ToList();
        File.WriteAllLines(fullPath, textLines);
    }
}
