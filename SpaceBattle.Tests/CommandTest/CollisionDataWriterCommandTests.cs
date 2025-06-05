namespace SpaceBattle.Lib.Tests.CommandTests;

using System;
using System.Collections.Generic;
using System.IO;
using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;
using Xunit;

public class CollisionDataWriterCommandTests : IDisposable
{
    private readonly string _testDir;
    private readonly object _scope;

    public CollisionDataWriterCommandTests()
    {
        // Initialize IoC
        new InitScopeBasedIoCImplementationCommand().Execute();

        // Create a new scope for tests
        var rootScope = IoC.Resolve<object>("Scopes.Root");
        _scope = IoC.Resolve<object>("Scopes.New", rootScope);
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        // Set up test directory
        _testDir = Path.Combine(Path.GetTempPath(), "SpaceBattleTests", Guid.NewGuid().ToString());
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }

        Directory.CreateDirectory(_testDir);

        // Register default storage directory
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.StorageDirectory",
            (object[] _) => _testDir
        ).Execute();
    }

    public void Dispose()
    {
        try
        {
            // Clean up test directory
            if (Directory.Exists(_testDir))
            {
                // Remove read-only attributes from all files and directories
                var directory = new DirectoryInfo(_testDir);
                foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
                {
                    file.Attributes = FileAttributes.Normal;
                }

                foreach (var dir in directory.GetDirectories("*", SearchOption.AllDirectories))
                {
                    dir.Attributes = FileAttributes.Normal;
                }

                Directory.Delete(_testDir, true);
            }

            // Reset the current scope
            if (_scope != null)
            {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            }
        }
        catch (Exception ex)
        {
            // Log the exception if needed
            Console.WriteLine($"Error during test cleanup: {ex}");
        }
    }

    [Fact]
    public void Constructor_WithNullFileName_ThrowsArgumentNullException()
    {
        var points = new List<int[]> { new[] { 1, 2, 3 } };
        Assert.Throws<ArgumentNullException>(() => new CollisionDataWriterCommand(null!, points));
    }

    [Fact]
    public void Constructor_WithNullCollisionPoints_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CollisionDataWriterCommand("test.log", null!));
    }

    [Fact]
    public void Execute_WithEmptyCollisionPoints_WritesEmptyFile()
    {
        // Arrange
        var fileName = "empty_test.log";
        var writer = new CollisionDataWriterCommand(fileName, new List<int[]>());

        // Act
        writer.Execute();

        // Assert
        var filePath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(filePath));
        var content = File.ReadAllText(filePath);
        Assert.Equal("", content.Trim());
    }

    [Fact]
    public void Execute_WithValidData_WritesCorrectContent()
    {
        // Arrange
        var fileName = "test.log";
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 }, new[] { 4, 5, 6 } };
        var writer = new CollisionDataWriterCommand(fileName, samplePoints);

        // Act
        writer.Execute();

        // Assert
        var filePath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(filePath));
        var lines = File.ReadAllLines(filePath);
        Assert.Equal(2, lines.Length);
        Assert.Equal("1,2,3", lines[0]);
        Assert.Equal("4,5,6", lines[1]);
    }

    [Fact]
    public void Execute_WithNestedDirectories_CreatesDirectories()
    {
        // Arrange
        var fileName = Path.Combine("nested", "dir", "test.log");
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var writer = new CollisionDataWriterCommand(fileName, samplePoints);

        // Act
        writer.Execute();

        // Assert
        var fullPath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(fullPath));
    }

    [Fact]
    public void Execute_WithNonexistentStorageDirectory_CreatesDirectoryAndWritesFile()
    {
        // Arrange
        var newDir = Path.Combine(_testDir, "nonexistent", "subdir");

        // Create a new scope for this test
        var scope = IoC.Resolve<object>("Scopes.New", _scope);
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Register the storage directory in the new scope
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => newDir
            ).Execute();

            var writer = new CollisionDataWriterCommand("test.log", new List<int[]> { new[] { 1, 2, 3 } });

            // Act - Should not throw
            var exception = Record.Exception(() => writer.Execute());
            Assert.Null(exception);

            // Assert - Verify file was created in the new directory
            var filePath = Path.Combine(newDir, "test.log");
            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath);
            Assert.Equal("1,2,3", content.Trim());
        }
        finally
        {
            // Reset to the test's scope
            IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();
        }
    }

    [Fact]
    public void Execute_WhenFileInUse_ThrowsInvalidOperation()
    {
        // Arrange
        var fileName = "in_use.log";
        var filePath = Path.Combine(_testDir, fileName);
        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            var writer = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } });

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains("Error writing to file", ex.Message);
        }
    }

    [Fact]
    public void Execute_WithVeryLongFileName_ThrowsInvalidOperation()
    {
        // Arrange
        var longFileName = new string('a', 260) + ".log";
        var writer = new CollisionDataWriterCommand(longFileName, new List<int[]> { new[] { 1, 2, 3 } });

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.True(ex.Message.Contains("Invalid file path or access denied") ||
                   ex.Message.Contains("Error writing to file"));
    }

    [Fact]
    public void Execute_WithInvalidFileName_ThrowsInvalidOperation()
    {
        // Arrange
        var invalidFileName = "test<>.log";
        var writer = new CollisionDataWriterCommand(invalidFileName, new List<int[]> { new[] { 1, 2, 3 } });

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.True(ex.Message.Contains("Invalid file path or access denied") ||
                   ex.Message.Contains("Error writing to file"));
    }

    [Fact]
    public void Execute_WithNullArrayInCollisionPoints_HandlesGracefully()
    {
        // Arrange
        var fileName = "null_array_test.log";
        var collisionPoints = new List<int[]> { new[] { 1, 2, 3 }, null!, new[] { 4, 5, 6 } };
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(_testDir);
        mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);

        var writer = new CollisionDataWriterCommand(fileName, collisionPoints, mockFileSystem.Object, mockDirProvider.Object);

        // Act
        writer.Execute();

        // Assert
        mockFileSystem.Verify(f => f.WriteAllLines(
            It.Is<string>(p => p.EndsWith(fileName)),
            It.Is<IEnumerable<string>>(lines =>
                lines.Count() == 3 &&
                lines.ElementAt(0) == "1,2,3" &&
                lines.ElementAt(1) == "" &&
                lines.ElementAt(2) == "4,5,6"
            )
        ), Times.Once);
    }

    [Fact]
    public void Execute_WithEmptyArrayInCollisionPoints_HandlesGracefully()
    {
        // Arrange
        var fileName = "empty_array_test.log";
        var collisionPoints = new List<int[]> { new[] { 1, 2, 3 }, Array.Empty<int>(), new[] { 4, 5, 6 } };
        var writer = new CollisionDataWriterCommand(fileName, collisionPoints);

        // Act
        writer.Execute();

        // Assert
        var filePath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(filePath));
        var lines = File.ReadAllLines(filePath);
        Assert.Equal(3, lines.Length);
        Assert.Equal("1,2,3", lines[0]);
        Assert.Equal("", lines[1]);
        Assert.Equal("4,5,6", lines[2]);
    }

    [Fact]
    public void Execute_WhenStorageDirectoryIsRoot_HandlesCorrectly()
    {
        // Arrange
        var rootDir = Path.GetPathRoot(Environment.CurrentDirectory) ?? throw new InvalidOperationException("Root directory cannot be null");
        var fileName = "root_test.log";

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => rootDir
            ).Execute();

            var writer = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } });

            // Act & Assert - Should either succeed or throw a specific exception
            var ex = Record.Exception(() => writer.Execute());

            if (ex != null)
            {
                Assert.True(ex is UnauthorizedAccessException ||
                           ex is IOException ||
                           ex is System.Security.SecurityException ||
                           (ex is InvalidOperationException &&
                            (ex.Message.Contains("Directory not found") ||
                             ex.Message.Contains("Invalid file path or access denied") ||
                             ex.Message.Contains("Error writing to file"))),
                          $"Unexpected exception type: {ex.GetType().Name} with message: {ex.Message}");
            }
            else
            {
                // If no exception, verify the file was created
                var fullPath = Path.Combine(rootDir, fileName);
                Assert.True(File.Exists(fullPath));
            }
        }
        finally
        {
            // Cleanup
            var filePath = Path.Combine(rootDir, fileName);
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch { }
            }

            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_WithNullStorageDirectory_ThrowsInvalidOperation()
    {
        // Arrange - Create a new scope with null storage directory
        var scope = IoC.Resolve<object>("Scopes.New", _scope);
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Explicitly register null as storage directory
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => null!)
            ).Execute();

            var writer = new CollisionDataWriterCommand("test.log", new List<int[]> { new[] { 1 } });

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains("Storage directory is not set", ex.Message);
        }
        finally
        {
            // Reset to the test's scope
            IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();
        }
    }

    [Fact]
    public void Execute_WithEmptyStorageDirectory_ThrowsInvalidOperation()
    {
        // Arrange - Create a new scope with empty storage directory
        var scope = IoC.Resolve<object>("Scopes.New", _scope);
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Register empty string as storage directory
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => string.Empty
            ).Execute();

            var writer = new CollisionDataWriterCommand("test.log", new List<int[]> { new[] { 1 } });

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains("Storage directory is not set", ex.Message);
        }
        finally
        {
            // Reset to the test's scope
            IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();
        }
    }

    [Fact]
    public void Execute_WithWhitespaceStorageDirectory_ThrowsInvalidOperation()
    {
        // Arrange - Create a new scope with whitespace storage directory
        var scope = IoC.Resolve<object>("Scopes.New", _scope);
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Register whitespace string as storage directory
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => "   "
            ).Execute();

            var writer = new CollisionDataWriterCommand("test.log", new List<int[]> { new[] { 1 } });

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            // The command should either throw about invalid path or about directory not being set
            Assert.True(ex.Message.Contains("Storage directory is not set") ||
                       ex.Message.Contains("Invalid file path or access denied"),
                      $"Unexpected error message: {ex.Message}");
        }
        finally
        {
            // Reset to the test's scope
            IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();
        }
    }
}
