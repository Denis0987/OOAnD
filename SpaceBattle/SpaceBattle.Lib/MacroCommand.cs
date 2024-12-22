namespace SpaceBattle.Lib;
public class MacroCommand : ICommand
{
    private readonly ICommand[] commands_;
    public MacroCommand(ICommand[] commands)
    {
        commands_ = commands;
    }
    public void Execute()
    {
        ExecuteCommands(0);
    }
    private void ExecuteCommands(int index)
    {
        if (index >= commands_.Length)
            return;

        commands_[index].Execute();
        ExecuteCommands(index + 1);
    }
}