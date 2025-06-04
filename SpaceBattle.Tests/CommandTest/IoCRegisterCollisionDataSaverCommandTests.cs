namespace SpaceBattle.Lib.Tests.CommandTests;

using System;
using System.Collections.Generic;
using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;
using Xunit;

public class IoCRegisterCollisionDataSaverCommandTests : IDisposable
{
    private readonly object _rootScope;
    private bool _disposed;
    private readonly object _testScope;

    public IoCRegisterCollisionDataSaverCommandTests()
    {
        try
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            _rootScope = IoC.Resolve<object>("Scopes.Root");

            _testScope = IoC.Resolve<object>("Scopes.New", _rootScope);
            IoC.Resolve<ICommand>("Scopes.Current.Set", _testScope).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "IoC.Scope.Current",
                (object[] _) => _testScope
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => "./collisions"
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.FileNameFormatter",
                (object[] _) => "formatted_name.log"
            ).Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in test setup: {ex}");
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                if (_rootScope != null)
                {
                    IoC.Resolve<ICommand>("Scopes.Current.Set", _rootScope).Execute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting IoC container: {ex}");
            }

            _disposed = true;
        }
    }

    [Fact]
    public void Execute_ShouldRegisterDataSaverStrategy()
    {
        new IoCRegisterCollisionDataSaverCommand().Execute();

        var fakeData = new List<int[]> { new[] { 5, 6, 7 } };
        var saverCmd = IoC.Resolve<ICommand>("Collision.DataSaver", "myfile.log", fakeData);

        Assert.IsType<CollisionDataWriterCommand>(saverCmd);
    }
}
