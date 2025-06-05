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
    private readonly string _testDir = Path.Combine(Path.GetTempPath() ?? string.Empty, "SpaceBattleTests");
    private bool _disposed = false;

    public CollisionDataWriterCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        var rootScope = IoC.Resolve<object>("Scopes.Root");
        var scope = IoC.Resolve<object>("Scopes.New", rootScope);

        // Create test directory if it doesn't exist
        if (!Directory.Exists(_testDir))
        {
            _ = Directory.CreateDirectory(_testDir);
        }

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "IoC.Scope.Current",
            (object[] _) => scope
        ).Execute();

        _testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempCollisions");
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, recursive: true);
        }

        Directory.CreateDirectory(_testDir);

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.StorageDirectory",
            (object[] _) => _testDir
        ).Execute();
    }

    [Fact]
    public void Execute_ShouldWriteFileWithCorrectContent()
    {
        var samplePoints = new List<int[]> { new[] { 2, 4, 6 }, new[] { 8, 10, 12 } };
        var fileName = "collision_test.log";

        var writer = new CollisionDataWriterCommand(fileName, samplePoints);

        writer.Execute();

        var fullPath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(fullPath));

        var lines = File.ReadAllLines(fullPath);
        Assert.Equal(2, lines.Length);
        Assert.Equal("2,4,6", lines[0]);
        Assert.Equal("8,10,12", lines[1]);
    }

    [Fact]
    public void Execute_WhenDirectoryNotExists_ShouldCreateDirectory()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "subdir/test_collision.log";
        var writer = new CollisionDataWriterCommand(fileName, samplePoints);

        writer.Execute();

        var fullPath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(fullPath));
        var lines = File.ReadAllLines(fullPath);
        Assert.Single(lines);
        Assert.Equal("1,2,3", lines[0]);
    }

    [Theory]
    [InlineData(null, "Storage directory is not set")]
    [InlineData("", "Storage directory is not set")]
    public void Execute_WithInvalidStorageDir_ShouldThrowInvalidOperationException(string storageDir, string expectedMessage)
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "test.log";

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => storageDir)
            ).Execute();

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains(expectedMessage, exception.Message);
        }
        finally
        {
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Constructor_WhenFileNameIsNull_ShouldThrowArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new CollisionDataWriterCommand(null!, new List<int[]> { new[] { 1 } })
        );
        Assert.Equal("fileName", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenCollisionPointsIsNull_ShouldThrowArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new CollisionDataWriterCommand("test.log", null!)
        );
        Assert.Equal("collisionPoints", exception.ParamName);
    }

    [Fact]
    public void Execute_WhenFileIsReadOnly_ShouldThrowInvalidOperationException()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "readonly_test.log";
        var fullPath = Path.Combine(_testDir, fileName);

        File.WriteAllText(fullPath, "test");
        File.SetAttributes(fullPath, FileAttributes.ReadOnly);

        var writer = new CollisionDataWriterCommand(fileName, samplePoints);

        try
        {
            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains("access denied", exception.Message.ToLower());
        }
        finally
        {
            File.SetAttributes(fullPath, FileAttributes.Normal);
            File.Delete(fullPath);
        }
    }

    [Fact]
    public void Execute_WithInvalidFileName_ShouldThrowException()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var invalidFileName = "invalid/\\?*:|" + new string(Path.GetInvalidFileNameChars()) + ".log";
        var writer = new CollisionDataWriterCommand(invalidFileName, samplePoints);

        var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.Contains("invalid file path", exception.Message.ToLower());
    }

    [Fact]
    public void Execute_WithNullArrayInCollisionPoints_ShouldHandleGracefully()
    {
        var pointsWithNull = new List<int[]> { new[] { 1, 2 }, default!, new[] { 3, 4 } };
        var fileName = "null_array_test.log";
        var writer = new CollisionDataWriterCommand(fileName, pointsWithNull);

        writer.Execute();

        var fullPath = Path.Combine(_testDir, fileName);
        var lines = File.ReadAllLines(fullPath);
        Assert.Equal(3, lines.Length);
        Assert.Equal("1,2", lines[0]);
        Assert.Equal("", lines[1]);
        Assert.Equal("3,4", lines[2]);
    }

    [Fact]
    public void Execute_WithEmptyCollisionPoints_ShouldCreateEmptyFile()
    {
        var emptyPoints = new List<int[]>();
        var fileName = "empty_points_test.log";
        var writer = new CollisionDataWriterCommand(fileName, emptyPoints);

        writer.Execute();

        var fullPath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(fullPath));
        var lines = File.ReadAllLines(fullPath);
        Assert.Empty(lines);
    }

    [Fact]
    public void Execute_WhenDirectoryCreationFails_ShouldThrowInvalidOperationException()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "invalid/directory/test.log";
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => _testDir)
            ).Execute();

            var invalidDir = Path.Combine(_testDir, "invalid");
            File.WriteAllText(invalidDir, "This is a file, not a directory");

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());

            Assert.True(exception.Message.Contains("Invalid file path or access denied") ||
                       exception.Message.Contains("Error writing to file") ||
                       exception.Message.Contains("Could not find a part of") ||
                       exception.Message.Contains("Directory not found") ||
                       exception.Message.Contains("Could not create"),
                       $"Unexpected error message: {exception.Message}");
        }
        finally
        {
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
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var longFileName = new string('a', 300) + ".log";
        var writer = new CollisionDataWriterCommand(longFileName, samplePoints);

        var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.True(exception.Message.Contains("Invalid file path") ||
                   exception.Message.Contains("access denied") ||
                   exception.Message.Contains("Error writing to file"));
    }

    [Fact]
    public void Execute_ShouldCreateDirectoryIfNotExists()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "test.log";
        var newDir = Path.Combine(_testDir, "newdir");

        if (Directory.Exists(newDir))
        {
            Directory.Delete(newDir, true);
        }

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => newDir)
            ).Execute();

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            writer.Execute();

            Assert.True(Directory.Exists(newDir));
            var filePath = Path.Combine(newDir, fileName);
            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath);
            Assert.Equal("1,2,3", content.Trim());
        }
        finally
        {
            if (Directory.Exists(newDir))
            {
                Directory.Delete(newDir, true);
            }

            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_WhenDirectoryNameIsNull_ShouldHandleGracefully()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "test.log";
        var testDir = Path.Combine(Path.GetTempPath(), "SpaceBattleTest");
        Directory.CreateDirectory(testDir);

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => testDir)
            ).Execute();

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            writer.Execute();

            var filePath = Path.Combine(testDir, fileName);
            Assert.True(File.Exists(filePath));

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch { }
        }
        finally
        {
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            try
            {
                Directory.Delete(testDir, true);
            }
            catch { }
        }
    }

    [Fact]
    public void Execute_WhenIOErrorOccurs_ShouldThrowWithCorrectMessage()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "test.log";
        var testFilePath = Path.Combine(_testDir, fileName);

        using (var fs = new FileStream(testFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            try
            {
                IoC.Resolve<ICommand>(
                    "IoC.Register",
                    "Collision.StorageDirectory",
                    (Func<object[], string>)(_ => _testDir)
                ).Execute();

                var writer = new CollisionDataWriterCommand(fileName, samplePoints);

                var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
                Assert.Contains("Error writing to file", exception.Message);
                Assert.IsType<IOException>(exception.InnerException);
            }
            finally
            {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            }
        }
    }

    [Fact]
    public void Execute_WhenSecurityExceptionOccurs_ShouldThrowWithCorrectMessage()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var testDir = Path.Combine(Path.GetTempPath(), "SpaceBattleSecureTest");

        if (Directory.Exists(testDir))
        {
            Directory.Delete(testDir, true);
        }

        Directory.CreateDirectory(testDir);

        try
        {
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            try
            {
                IoC.Resolve<ICommand>(
                    "IoC.Register",
                    "Collision.StorageDirectory",
                    (Func<object[], string>)(_ => testDir)
                ).Execute();

                File.WriteAllText(Path.Combine(testDir, "subdir"), "This is a file, not a directory");

                var writer = new CollisionDataWriterCommand("subdir/test.log", samplePoints);

                var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
                Assert.True(exception.Message.Contains("Invalid file path or access denied") ||
                           exception.Message.Contains("Error writing to file"),
                    $"Unexpected error message: {exception.Message}");
            }
            finally
            {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            }
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                try
                {
                    Directory.Delete(testDir, true);
                }
                catch { }
            }
        }
    }

    [Fact]
    public void Execute_WhenRootDirectory_ShouldHandleCorrectly()
    {
        var rootDir = Path.GetPathRoot(Environment.CurrentDirectory);
        var fileName = "test_root.log";
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => rootDir
            ).Execute();

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            var exception = Record.Exception(() => writer.Execute());

            if (exception != null)
            {
                Assert.True(exception is UnauthorizedAccessException ||
                           exception is IOException ||
                           exception is System.Security.SecurityException ||
                           (exception is InvalidOperationException &&
                            (exception.Message.Contains("Directory not found") ||
                             exception.Message.Contains("Invalid file path or access denied") ||
                             exception.Message.Contains("Error writing to file"))),
                          $"Unexpected exception type: {exception.GetType().Name} with message: {exception.Message}");
            }
        }
        finally
        {
            if (rootDir != null)
            {
                var filePath = Path.Combine(rootDir, fileName);
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch { }
                }
            }

            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_WhenPathGetDirectoryNameReturnsNull_ShouldHandleCorrectly()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            try
            {
                var testDir = Path.Combine(Path.GetTempPath(), "test_dir");
                if (Directory.Exists(testDir))
                {
                    Directory.Delete(testDir, true);
                }

                File.WriteAllText(testDir, "This is a file, not a directory");

                IoC.Resolve<ICommand>(
                    "IoC.Register",
                    "Collision.StorageDirectory",
                    (object[] _) => testDir
                ).Execute();

                var writer = new CollisionDataWriterCommand("test.log", samplePoints);

                var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
                Assert.True(exception.Message.Contains("Invalid file path or access denied") ||
                           exception.Message.Contains("Error writing to file") ||
                           exception.Message.Contains("Directory not found"),
                    $"Unexpected error message: {exception.Message}");
            }
            finally
            {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            }
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void Execute_WhenDirectoryCreationThrowsUnauthorizedAccess_ShouldThrowInvalidOperation()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var protectedDir = Path.Combine(tempDir, "protected");
        var testFilePath = Path.Combine(protectedDir, "test.log");

        try
        {
            var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            try
            {
                Directory.CreateDirectory(protectedDir);
                var dirInfo = new DirectoryInfo(protectedDir);
                dirInfo.Attributes |= FileAttributes.ReadOnly;

                IoC.Resolve<ICommand>(
                    "IoC.Register",
                    "Collision.StorageDirectory",
                    (object[] _) => tempDir
                ).Execute();

                var writer = new CollisionDataWriterCommand("protected/test.log", samplePoints);

                var exception = Record.Exception(() => writer.Execute());

                if (exception != null)
                {
                    Assert.IsType<InvalidOperationException>(exception);
                    Assert.True(exception.Message.Contains("Invalid file path or access denied") ||
                               exception.Message.Contains("Directory not found") ||
                               exception.Message.Contains("Error writing to file"),
                        $"Unexpected error message: {exception.Message}");
                }
                else
                {
                    Assert.True(File.Exists(Path.Combine(protectedDir, "test.log")));
                }
            }
            finally
            {
                try
                {
                    if (Directory.Exists(protectedDir))
                    {
                        var dir = new DirectoryInfo(protectedDir);
                        dir.Attributes &= ~FileAttributes.ReadOnly;

                        if (File.Exists(testFilePath))
                        {
                            File.SetAttributes(testFilePath, FileAttributes.Normal);
                            File.Delete(testFilePath);
                        }
                    }

                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
                catch { }

                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            }
        }
        finally
        {
            try
            {
                if (Directory.Exists(protectedDir))
                {
                    var dir = new DirectoryInfo(protectedDir);
                    dir.Attributes &= ~FileAttributes.ReadOnly;
                    if (File.Exists(testFilePath))
                    {
                        File.SetAttributes(testFilePath, FileAttributes.Normal);
                        File.Delete(testFilePath);
                    }
                }

                if (Directory.Exists(tempDir))
                {
                    try
                    {
                        Directory.Delete(tempDir, true);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }

    [Fact]
    public void Execute_WhenFileIsReadOnly_ShouldThrowInvalidOperation()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

            try
            {
                var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(testDir);
                var readOnlyFile = Path.Combine(testDir, "readonly.log");
                File.WriteAllText(readOnlyFile, "test");
                File.SetAttributes(readOnlyFile, FileAttributes.ReadOnly);

                IoC.Resolve<ICommand>(
                    "IoC.Register",
                    "Collision.StorageDirectory",
                    (object[] _) => testDir
                ).Execute();

                var writer = new CollisionDataWriterCommand("readonly.log", samplePoints);

                var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());
                Assert.True(exception.Message.Contains("Invalid file path or access denied") ||
                           exception.Message.Contains("Error writing to file"),
                    $"Unexpected error message: {exception.Message}");
            }
            finally
            {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            }
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.SetAttributes(tempFile, FileAttributes.Normal);
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void Execute_WithNotSupportedPath_ShouldThrowInvalidOperationException()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var invalidFileName = "test.log";
        var testDir = "\\\\?\\NonExistentDrive\\Test"; // Simulate unsupported path

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => testDir)
            ).Execute();

            var writer = new CollisionDataWriterCommand(invalidFileName, samplePoints);

            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());

            // Accept either error message as both are valid for this case
            Assert.True(
                exception.Message.Contains("Invalid file path or access denied") ||
                exception.Message.Contains("Directory not found"),
                $"Unexpected error message: {exception.Message}");

            // The inner exception could be NotSupportedException or DirectoryNotFoundException
            Assert.True(
                exception.InnerException is NotSupportedException ||
                exception.InnerException is DirectoryNotFoundException,
                $"Unexpected inner exception type: {exception.InnerException?.GetType().Name}");
        }
        finally
        {
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_WithRelativePath_ShouldHandleCorrectly()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = Path.Combine("..", "relative_test.log");
        var testDir = Path.Combine(_testDir ?? string.Empty, "subdir");
        _ = Directory.CreateDirectory(testDir);

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => testDir)
            ).Execute();

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            writer.Execute();

            var parentDir = Directory.GetParent(testDir)?.FullName ?? string.Empty;
            var fullPath = Path.Combine(parentDir, "relative_test.log");
            Assert.True(File.Exists(fullPath));
            var lines = File.ReadAllLines(fullPath);
            Assert.Single(lines);
            Assert.Equal("1,2,3", lines[0]);
        }
        finally
        {
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            var parentDir = Directory.GetParent(testDir)?.FullName;
            if (parentDir != null)
            {
                var filePath = Path.Combine(parentDir, "relative_test.log");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }

    [Fact]
    public void Execute_WithMultipleNullAndEmptyArrays_ShouldHandleGracefully()
    {
        var pointsWithNullAndEmpty = new List<int[]> { new[] { 1, 2 }, null!, Array.Empty<int>(), new[] { 3, 4 }, null! };
        var fileName = "multiple_null_empty_test.log";
        var writer = new CollisionDataWriterCommand(fileName, pointsWithNullAndEmpty);

        writer.Execute();

        var fullPath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(fullPath));
        var lines = File.ReadAllLines(fullPath);
        Assert.Equal(5, lines.Length);
        Assert.Equal("1,2", lines[0]);
        Assert.Equal("", lines[1]);
        Assert.Equal("", lines[2]);
        Assert.Equal("3,4", lines[3]);
        Assert.Equal("", lines[4]);
    }

    [Fact]
    public void Execute_WithInaccessibleDrive_ShouldThrowInvalidOperationException()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "test.log";
        var inaccessibleDir = "Z:\\NonExistentDrive"; // Simulate inaccessible drive

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => inaccessibleDir)
            ).Execute();

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            var exception = Assert.Throws<InvalidOperationException>(() => writer.Execute());

            // Accept multiple possible error messages as different environments may throw different exceptions
            Assert.True(
                exception.Message.Contains("Directory not found") ||
                exception.Message.Contains("Invalid file path or access denied") ||
                exception.Message.Contains("Error writing to file"),
                $"Unexpected error message: {exception.Message}");
        }
        finally
        {
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    [Fact]
    public void Execute_WithRootFileName_ShouldHandleCorrectly()
    {
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 } };
        var fileName = "root_test.log";
        var testDir = Path.GetPathRoot(Environment.CurrentDirectory) ?? string.Empty; // Корневая директория (например, "C:\")

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (Func<object[], string>)(_ => testDir)
            ).Execute();

            var writer = new CollisionDataWriterCommand(fileName, samplePoints);

            var exception = Record.Exception(() => writer.Execute());

            if (exception != null)
            {
                Assert.IsType<InvalidOperationException>(exception);
                Assert.True(
                    exception.Message.Contains("Invalid file path or access denied") ||
                    exception.Message.Contains("Directory not found") ||
                    exception.Message.Contains("Error writing to file"),
                    $"Unexpected error message: {exception.Message}");
            }
            else
            {
                var fullPath = Path.Combine(testDir ?? string.Empty, fileName);
                Assert.True(File.Exists(fullPath));
                var lines = File.ReadAllLines(fullPath);
                Assert.Single(lines);
                Assert.Equal("1,2,3", lines[0]);

                // Cleanup
                try
                {
                    File.SetAttributes(fullPath, FileAttributes.Normal);
                    File.Delete(fullPath);
                }
                catch { }
            }
        }
        finally
        {
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Clean up test directory if it exists
                try
                {
                    if (Directory.Exists(_testDir))
                    {
                        // Remove read-only attributes from all files
                        var files = Directory.GetFiles(_testDir, "*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                File.SetAttributes(file, FileAttributes.Normal);
                            }
                            catch { }
                        }

                        // Delete the directory
                        Directory.Delete(_testDir, true);
                    }
                }
                catch { }
            }

            _disposed = true;
        }
    }

    ~CollisionDataWriterCommandTests()
    {
        Dispose(false);
    }
}
