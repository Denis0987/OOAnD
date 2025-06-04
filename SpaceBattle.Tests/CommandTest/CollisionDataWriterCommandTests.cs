namespace SpaceBattle.Lib.Tests.CommandTests;

using System;
using System.Collections.Generic;
using System.IO;
using SpaceBattle.Lib.Commands;
using Xunit;

public class CollisionDataWriterCommandTests
{
    private readonly string _testDir;

    public CollisionDataWriterCommandTests()
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

            // Create a temporary directory for tests
            _testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempCollisions");
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, recursive: true);
            }

            Directory.CreateDirectory(_testDir);

            // Register the storage directory for these tests
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => _testDir
            ).Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in test setup: {ex}");
            throw;
        }
    }

    [Fact]
    public void Execute_ShouldWriteFileWithCorrectContent()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 2, 4, 6 }, new[] { 8, 10, 12 } };
        var fileName = "collision_test.log";

        var writer = new CollisionDataWriterCommand(fileName, samplePoints);

        // Act
        writer.Execute();

        // Assert: файл по ожидаемому пути должен существовать
        var fullPath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(fullPath));

        // Содержимое: каждая строка — числа, разделённые запятой
        var lines = File.ReadAllLines(fullPath);
        Assert.Equal(2, lines.Length);
        Assert.Equal("2,4,6", lines[0]);
        Assert.Equal("8,10,12", lines[1]);
    }

    [Fact]
    public void Execute_WhenDirectoryNotExists_ShouldCreateDirectory()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "subdir/test_collision.log";
        var writer = new CollisionDataWriterCommand(fileName, samplePoints);

        // Act
        writer.Execute();

        // Assert
        var fullPath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(fullPath));
        var lines = File.ReadAllLines(fullPath);
        Assert.Single(lines);
        Assert.Equal("1,2,3", lines[0]);
    }

    [Fact]
    public void Execute_WhenStorageDirNotSet_ShouldThrowException()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };

        // Create a new scope to avoid affecting other tests
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Register a null storage directory
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string?>)(_ => null)
            ).Execute();

            var writer = new CollisionDataWriterCommand("test.log", samplePoints);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains("Storage directory is not set", exception.Message);
        }
        finally
        {
            // Restore the original scope
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Constructor_WhenFileNameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new CollisionDataWriterCommand(null!, new List<int[]> { new[] { 1 } })
        );
        Assert.Equal("fileName", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenCollisionPointsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new CollisionDataWriterCommand("test.log", null!)
        );
        Assert.Equal("collisionPoints", exception.ParamName);
    }
}
