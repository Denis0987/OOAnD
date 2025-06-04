namespace SpaceBattle.Lib.Tests.CommandTests;

using System.Collections.Generic;
using SpaceBattle.Lib.Commands;
using Xunit;

public class IoCRegisterCollisionDataSaverCommandTests
{
    public IoCRegisterCollisionDataSaverCommandTests()
    {
        try
        {
            // Get the root scope
            var rootScope = IoC.Resolve<object>("Scopes.Root");

            // Create a new scope for the test
            var scope = IoC.Resolve<object>("Scopes.New", rootScope);
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            // Register required dependencies
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "IoC.Scope.Current",
                (object[] _) => scope
            ).Execute();

            // Register storage directory
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => "./collisions"
            ).Execute();

            // Register file name formatter
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

    [Fact]
    public void Execute_ShouldRegisterDataSaverStrategy()
    {
        // Act: регистрируем стратегию сохранения
        new IoCRegisterCollisionDataSaverCommand().Execute();

        // Теперь проверим, что по ключу "Collision.DataSaver" IoC выдаст команду нужного типа
        var fakeData = new List<int[]> { new[] { 5, 6, 7 } };
        var saverCmd = IoC.Resolve<ICommand>("Collision.DataSaver", "myfile.log", fakeData);

        // Убедимся, что получили экземпляр CollisionDataWriterCommand
        Assert.IsType<CollisionDataWriterCommand>(saverCmd);
    }
}
