namespace SpaceBattle.Lib.Tests.CommandTests;

using System;
using System.Collections.Generic;
using System.IO;
using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;
using Xunit;

public class CollisionDataWriterCommandTests
{
    private readonly string _testDir;

    public CollisionDataWriterCommandTests()
    {
        try
        {
            // Initialize the IoC container implementation
            new InitScopeBasedIoCImplementationCommand().Execute();

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

    [Fact]
    public void Execute_WhenFileIsReadOnly_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "readonly_test.log";
        var fullPath = Path.Combine(_testDir, fileName);

        // Create a read-only file
        File.WriteAllText(fullPath, "test");
        File.SetAttributes(fullPath, FileAttributes.ReadOnly);

        var writer = new CollisionDataWriterCommand(fileName, samplePoints);

        try
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains("access denied", exception.Message.ToLower());
        }
        finally
        {
            // Cleanup
            File.SetAttributes(fullPath, FileAttributes.Normal);
            File.Delete(fullPath);
        }
    }

    [Fact]
    public void Execute_WithInvalidFileName_ShouldThrowException()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        // Use a path with invalid characters that will be rejected by Path.Combine
        var invalidFileName = "invalid/\\?*:|" + new string(Path.GetInvalidFileNameChars()) + ".log";
        var writer = new CollisionDataWriterCommand(invalidFileName, samplePoints);

        // Act & Assert - Verify that an InvalidOperationException is thrown with the correct message
        var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.Contains("invalid file path", exception.Message.ToLower());
    }

    [Fact]
    public void Execute_WithNullFileName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new CollisionDataWriterCommand(null!, samplePoints)
        );
        Assert.Equal("fileName", exception.ParamName);
    }

    [Fact]
    public void Execute_WithNullArrayInCollisionPoints_ShouldHandleGracefully()
    {
        // Arrange
        var pointsWithNull = new List<int[]> { new[] { 1, 2 }, default!, new[] { 3, 4 } };
        var fileName = "null_array_test.log";
        var writer = new CollisionDataWriterCommand(fileName, pointsWithNull);

        // Act
        writer.Execute();

        // Assert - null arrays should be handled gracefully (converted to empty strings)
        var fullPath = Path.Combine(_testDir, fileName);
        var lines = File.ReadAllLines(fullPath);
        Assert.Equal(3, lines.Length);
        Assert.Equal("1,2", lines[0]);
        Assert.Equal("", lines[1]); // Empty string for null array
        Assert.Equal("3,4", lines[2]);
    }

    [Fact]
    public void Execute_WithEmptyCollisionPoints_ShouldCreateEmptyFile()
    {
        // Arrange
        var emptyPoints = new List<int[]>();
        var fileName = "empty_points_test.log";
        var writer = new CollisionDataWriterCommand(fileName, emptyPoints);

        // Act
        writer.Execute();

        // Assert - file should be created but empty
        var fullPath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(fullPath));
        var lines = File.ReadAllLines(fullPath);
        Assert.Empty(lines);
    }

    [Fact]
    public void Execute_WhenDirectoryCreationFails_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "invalid/directory/test.log";
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Register test directory in IoC
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => _testDir)
            ).Execute();

            // Create a file with the same name as the directory we want to create
            var invalidDir = Path.Combine(_testDir, "invalid");
            File.WriteAllText(invalidDir, "This is a file, not a directory");

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            // Act & Assert - Should fail when trying to create a directory where a file exists
            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());

            // Accept multiple possible error messages as they can vary by OS or .NET version
            Assert.True(exception.Message.Contains("Invalid file path or access denied") ||
                       exception.Message.Contains("Error writing to file") ||
                       exception.Message.Contains("Could not find a part of") ||
                       exception.Message.Contains("Directory not found") ||
                       exception.Message.Contains("Could not create"),
                       $"Unexpected error message: {exception.Message}");
        }
        finally
        {
            // Cleanup
            var invalidDir = Path.Combine(_testDir, "invalid");
            if (File.Exists(invalidDir))
            {
                File.Delete(invalidDir);
            }

            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_WithPathTooLong_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var longFileName = new string('a', 300) + ".log"; // Very long filename
        var writer = new CollisionDataWriterCommand(longFileName, samplePoints);

        // Act & Assert - Should throw due to path being too long
        var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.True(exception.Message.Contains("Invalid file path") ||
                   exception.Message.Contains("access denied") ||
                   exception.Message.Contains("Error writing to file"));
    }

    [Fact]
    public void Execute_WithEmptyStorageDirectory_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Register empty storage directory
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => string.Empty)
            ).Execute();

            var writer = new CollisionDataWriterCommand("test.log", samplePoints);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains("Storage directory is not set", exception.Message);
        }
        finally
        {
            // Cleanup
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_WithNullStorageDir_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Register null storage directory
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
            // Cleanup
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_WithEmptyStorageDir_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Register empty storage directory
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => string.Empty)
            ).Execute();

            var writer = new CollisionDataWriterCommand("test.log", samplePoints);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains("Storage directory is not set", exception.Message);
        }
        finally
        {
            // Cleanup
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_ShouldCreateDirectoryIfNotExists()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "test.log";
        var newDir = Path.Combine(_testDir, "newdir");

        // Ensure the directory doesn't exist before test
        if (Directory.Exists(newDir))
        {
            Directory.Delete(newDir, true);
        }

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            // Register new directory that will be created
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => newDir)
            ).Execute();

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            // Act
            writer.Execute();

            // Assert
            Assert.True(Directory.Exists(newDir), "Directory should be created");
            var filePath = Path.Combine(newDir, fileName);
            Assert.True(File.Exists(filePath), "File should be created");
            var content = File.ReadAllText(filePath);
            Assert.Equal("1,2,3", content.Trim());
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(newDir))
            {
                Directory.Delete(newDir, true);
            }

            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_WhenIOErrorOccurs_ShouldThrowWithCorrectMessage()
    {
        // Arrange
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "test.log";
        var testFilePath = Path.Combine(_testDir, fileName);

        // Create a file and lock it to cause an IOError
        using (var fs = new FileStream(testFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            try
            {
                // Register test directory
                IoC.Resolve<ICommand>(
                    "IoC.Register",
                    "Collision.StorageDirectory",
                    (Func<object[], string>)(_ => _testDir)
                ).Execute();

                var writer = new CollisionDataWriterCommand(fileName, samplePoints);

                // Act & Assert
                var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
                Assert.Contains("Error writing to file", exception.Message);
                Assert.IsType<IOException>(exception.InnerException);
            }
            finally
            {
                // Cleanup
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            }
        }
    }
}
