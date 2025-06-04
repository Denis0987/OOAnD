namespace SpaceBattle.Lib.Commands;

using System;
using System.Collections.Generic;

public class IoCRegisterCollisionFileNameFormatterCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.FileNameFormatter",
            (object[] args) =>
            {
                if (args == null || args.Length < 2)
                {
                    throw new ArgumentException("Two arguments are required");
                }

                var first = args[0]?.ToString() ?? throw new ArgumentException("First argument cannot be null");
                var second = args[1]?.ToString() ?? throw new ArgumentException("Second argument cannot be null");
                
                return $"{first}__{second}.log";
            }
        ).Execute();
    }
}
