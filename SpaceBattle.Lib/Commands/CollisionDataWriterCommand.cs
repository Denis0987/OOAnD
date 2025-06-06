namespace SpaceBattle.Lib;

public class SaveCollisionDataCommand : ICommand
{
    private readonly string _targetFile;
    private readonly IList<int[]> _impactData;

    public SaveCollisionDataCommand(string targetFile, IList<int[]> impactData)
    {
        _targetFile = targetFile;
        _impactData = impactData;
    }

    public void Execute()
    {
        var savePath = IoC.Resolve<string>("Collision.StoragePath");
        var fullPath = Path.Combine(savePath, _targetFile);
        var textLines = _impactData.Select(vector => string.Join(" ", vector)).ToList();
        File.WriteAllLines(fullPath, textLines);
    }
}
