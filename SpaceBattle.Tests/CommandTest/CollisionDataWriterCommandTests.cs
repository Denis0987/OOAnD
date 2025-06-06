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
        new InitScopeBasedIoCImplementationCommand().Execute();
        var rootScope = IoC.Resolve<object>("Scopes.Root");
        _scope = IoC.Resolve<object>("Scopes.New", rootScope);
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        _testDir = Path.Combine(Path.GetTempPath(), "SpaceBattleTests", Guid.NewGuid().ToString());
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }

        Directory.CreateDirectory(_testDir);

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
            if (Directory.Exists(_testDir))
            {
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

            if (_scope != null)
            {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Root")).Execute();
            }
        }
        catch (Exception ex)
        {
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
        var fileName = "empty_test.log";
        var writer = new CollisionDataWriterCommand(fileName, new List<int[]>());

        writer.Execute();

        var filePath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(filePath));
        var content = File.ReadAllText(filePath);
        Assert.Equal("", content.Trim());
    }

    [Fact]
    public void Execute_WithValidData_WritesCorrectContent()
    {
        var fileName = "test.log";
        var samplePoints = new List<int[]> { new[] { 1, 2, 3 }, new[] { 4, 5, 6 } };
        var writer = new CollisionDataWriterCommand(fileName, samplePoints);

        writer.Execute();

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
        var directory = Path.Combine("nested", "dir");
        var fileName = "test.log";
        var fullPath = Path.Combine(_testDir, directory, fileName);

        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(_testDir);
        mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(false);
        mockFileSystem.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Callback<string, IEnumerable<string>>((path, _) =>
            {
                Assert.Contains(directory, path);
                Assert.EndsWith(fileName, path);
            });

        var writer = new CollisionDataWriterCommand(
            Path.Combine(directory, fileName),
            new List<int[]> { new[] { 1, 2, 3 } },
            mockFileSystem.Object,
            mockDirProvider.Object);

        writer.Execute();

        mockFileSystem.Verify(f => f.CreateDirectory(It.Is<string>(d => d.Contains(directory))), Times.Once);
        mockFileSystem.Verify(f => f.WriteAllLines(It.Is<string>(p => p.Contains(directory) && p.EndsWith(fileName)),
            It.IsAny<IEnumerable<string>>()), Times.Once);
    }

    [Fact]
    public void Execute_WithNonexistentStorageDirectory_CreatesDirectoryAndWritesFile()
    {
        var newDir = Path.Combine(_testDir, "nonexistent", "subdir");

        var scope = IoC.Resolve<object>("Scopes.New", _scope);
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        try
        {
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.StorageDirectory",
                (object[] _) => newDir
            ).Execute();

            var writer = new CollisionDataWriterCommand("test.log", new List<int[]> { new[] { 1, 2, 3 } });

            var exception = Record.Exception(() => writer.Execute());
            Assert.Null(exception);

            var filePath = Path.Combine(newDir, "test.log");
            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath);
            Assert.Equal("1,2,3", content.Trim());
        }
        finally
        {
            IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();
        }
    }

    [Fact]
    public void Execute_WhenFileInUse_ThrowsInvalidOperation()
    {
        var fileName = "in_use.log";
        var filePath = Path.Combine(_testDir, fileName);
        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            var writer = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } });

            var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
            Assert.Contains("Error writing to file", ex.Message);
        }
    }

    [Fact]
    public void Execute_WithVeryLongFileName_ThrowsInvalidOperation()
    {
        var longFileName = new string('a', 260) + ".log";
        var writer = new CollisionDataWriterCommand(longFileName, new List<int[]> { new[] { 1, 2, 3 } });

        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.True(ex.Message.Contains("Invalid file path or access denied") ||
                   ex.Message.Contains("Error writing to file"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyOrWhitespaceFileName_ThrowsArgumentException(string fileName)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } }));
        Assert.Contains("File name cannot be empty or whitespace", ex.Message);
        Assert.Equal("fileName", ex.ParamName);
    }

    [Theory]
    [InlineData("CON")]
    [InlineData("PRN")]
    [InlineData("AUX")]
    [InlineData("NUL")]
    [InlineData("COM1")]
    [InlineData("LPT1")]
    public void Constructor_WithReservedFileName_ThrowsArgumentException(string fileName)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new CollisionDataWriterCommand($"{fileName}.log", new List<int[]> { new[] { 1, 2, 3 } }));
        Assert.Contains("File name is a reserved system name", ex.Message);
        Assert.Equal("fileName", ex.ParamName);
    }

    [Theory]
    [InlineData("test<>.log")]
    [InlineData("test|.log")]
    [InlineData("test?.log")]
    [InlineData("test*.log")]
    [InlineData("test\".log")]
    [InlineData("test:.log")]
    [InlineData("test ")]
    [InlineData("test.")]
    [InlineData("test..")]
    [InlineData(".")]
    [InlineData("..")]
    public void Constructor_WithInvalidFileName_ThrowsArgumentException(string fileName)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } }));
        Assert.Contains("File name contains invalid characters", ex.Message);
        Assert.Equal("fileName", ex.ParamName);
    }

    [Theory]
    [InlineData("test/.log")]
    [InlineData("test\\.log")]
    public void Constructor_WithDirectorySeparators_DoesNotThrow(string fileName)
    {
        // These should not throw as the implementation handles directory separators
        var command = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } });
        Assert.NotNull(command);
    }

    [Fact]
    public void Execute_WithNullStorageDirectory_ThrowsInvalidOperation()
    {
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns((string)null!);

        var writer = new CollisionDataWriterCommand("test.log", new List<int[]> { new[] { 1, 2, 3 } },
            mockFileSystem.Object, mockDirProvider.Object);

        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.Equal("Storage directory is not set", ex.Message);
    }

    [Fact]
    public void Execute_WithEmptyStorageDirectory_ThrowsInvalidOperation()
    {
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns("");

        var writer = new CollisionDataWriterCommand("test.log", new List<int[]> { new[] { 1, 2, 3 } },
            mockFileSystem.Object, mockDirProvider.Object);

        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.Equal("Storage directory is not set", ex.Message);
    }

    [Fact]
    public void Execute_WithWhitespaceStorageDirectory_ThrowsInvalidOperation()
    {
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns("   ");

        var writer = new CollisionDataWriterCommand("test.log", new List<int[]> { new[] { 1, 2, 3 } },
            mockFileSystem.Object, mockDirProvider.Object);

        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.Equal("Storage directory is not set", ex.Message);
    }

    [Fact]
    public void Execute_WithNullArrayInCollisionPoints_HandlesGracefully()
    {
        var fileName = "null_array_test.log";
        var collisionPoints = new List<int[]> { new[] { 1, 2, 3 }, null!, new[] { 4, 5, 6 } };
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(_testDir);
        mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);

        var writer = new CollisionDataWriterCommand(fileName, collisionPoints, mockFileSystem.Object, mockDirProvider.Object);

        writer.Execute();

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
    public void Execute_WhenFileSystemThrowsUnauthorizedAccess_ThrowsInvalidOperation()
    {
        var fileName = "unauthorized.log";
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(_testDir);
        mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);
        mockFileSystem.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Throws<UnauthorizedAccessException>();

        var writer = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } },
            mockFileSystem.Object, mockDirProvider.Object);

        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.Contains("Invalid file path or access denied", ex.Message);
        Assert.IsType<UnauthorizedAccessException>(ex.InnerException);
    }

    [Fact]
    public void Execute_WhenDiskIsFull_ThrowsInvalidOperation()
    {
        var fileName = "disk_full_test.log";
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(_testDir);
        mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);

        var ioException = new IOException("Insufficient disk space", new IOException("The disk is full"));
        mockFileSystem.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Throws(ioException);

        var writer = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } },
            mockFileSystem.Object, mockDirProvider.Object);

        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.Contains("Error writing to file", ex.Message);
        Assert.Same(ioException, ex.InnerException);
    }

    [Fact]
    public void Execute_WithCustomFileSystemAndDirectoryProvider_WorksCorrectly()
    {
        var fileName = "custom_impl_test.log";
        var expectedLines = new[] { "1,2,3", "4,5,6" };
        var collisionPoints = new List<int[]> { new[] { 1, 2, 3 }, new[] { 4, 5, 6 } };

        var customFileSystem = new CustomFileSystem();
        var customDirProvider = new CustomDirectoryProvider(_testDir);

        var writer = new CollisionDataWriterCommand(fileName, collisionPoints, customFileSystem, customDirProvider);

        writer.Execute();

        var filePath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(filePath));
        var actualLines = File.ReadAllLines(filePath);
        Assert.Equal(expectedLines, actualLines);
    }

    [Fact]
    public void Execute_WithLargeNumberOfCollisionPoints_HandlesCorrectly()
    {
        var fileName = "large_file_test.log";
        var random = new Random();
        const int pointCount = 10000;

        var collisionPoints = new List<int[]>();
        var expectedLines = new List<string>();

        for (var i = 0; i < pointCount; i++)
        {
            var x = random.Next(1000);
            var y = random.Next(1000);
            var z = random.Next(1000);
            var point = new[] { x, y, z };
            collisionPoints.Add(point);
            expectedLines.Add($"{x},{y},{z}");
        }

        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(_testDir);
        mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);

        IEnumerable<string>? actualLines = null;
        mockFileSystem.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Callback<string, IEnumerable<string>>((_, lines) => actualLines = lines.ToList());

        var writer = new CollisionDataWriterCommand(fileName, collisionPoints,
            mockFileSystem.Object, mockDirProvider.Object);

        writer.Execute();

        Assert.NotNull(actualLines);
        Assert.Equal(pointCount, actualLines!.Count());
        Assert.Equal(expectedLines[0], actualLines!.First());
        Assert.Equal(expectedLines[pointCount - 1], actualLines!.Last());
        Assert.Equal(expectedLines, actualLines);
    }

    [Fact]
    public void Execute_WhenDirectoryCreationFails_ThrowsInvalidOperation()
    {
        // Arrange
        var fileName = "directory_creation_fail.log";
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        var testDir = Path.Combine(_testDir, "nonexistent");
        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(testDir);

        // Simulate directory doesn't exist and creation fails
        mockFileSystem.Setup(f => f.DirectoryExists(testDir)).Returns(false);
        mockFileSystem.Setup(f => f.CreateDirectory(testDir))
            .Throws(new UnauthorizedAccessException("Access to the path is denied"));

        var writer = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } },
            mockFileSystem.Object, mockDirProvider.Object);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.Contains("Invalid file path or access denied", ex.Message);
        Assert.IsType<UnauthorizedAccessException>(ex.InnerException);

        // Verify WriteAllLines was never called since directory creation failed
        mockFileSystem.Verify(
            f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()),
            Times.Never);
    }

    [Fact]
    public void Execute_WhenDirectoryIsReadOnly_ThrowsInvalidOperation()
    {
        // Arrange
        var fileName = "readonly_test.log";
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        var testDir = Path.Combine(_testDir, "readonly_dir");
        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(testDir);

        // Directory exists but is read-only
        mockFileSystem.Setup(f => f.DirectoryExists(testDir)).Returns(true);
        mockFileSystem.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Throws(new UnauthorizedAccessException("Access to the path is denied"));

        var writer = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } },
            mockFileSystem.Object, mockDirProvider.Object);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => writer.Execute());
        Assert.Contains("Invalid file path or access denied", ex.Message);
        Assert.IsType<UnauthorizedAccessException>(ex.InnerException);
    }

    [Fact]
    public void Execute_WithEmptyArrayInCollisionPoints_HandlesGracefully()
    {
        var fileName = "empty_array_test.log";
        var collisionPoints = new List<int[]> { new[] { 1, 2, 3 }, Array.Empty<int>(), new[] { 4, 5, 6 } };
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(_testDir);
        mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);

        var writer = new CollisionDataWriterCommand(fileName, collisionPoints, mockFileSystem.Object, mockDirProvider.Object);

        writer.Execute();

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
    public void Execute_WithSpecialCharactersInFileName_WorksCorrectly()
    {
        var fileName = "test_äöüß_测试_тест_😊.log";
        var writer = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } });

        writer.Execute();

        var filePath = Path.Combine(_testDir, fileName);
        Assert.True(File.Exists(filePath));
        var content = File.ReadAllLines(filePath);
        Assert.Single(content);
        Assert.Equal("1,2,3", content[0]);
    }

    [Fact]
    public void Execute_WithDifferentDirectorySeparators_WorksCorrectly()
    {
        var fileName = "dir1/dir2\\dir3/test.log";
        var writer = new CollisionDataWriterCommand(fileName, new List<int[]> { new[] { 1, 2, 3 } });

        writer.Execute();

        var normalizedPath = Path.Combine(_testDir, fileName.Replace('\\', Path.DirectorySeparatorChar)
                                                          .Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(normalizedPath));
    }

    [Fact]
    public void Execute_WhenPathIsRoot_HandlesCorrectly()
    {
        // This test verifies behavior when the path is at the root of the drive
        // Arrange
        var mockFileSystem = new Mock<CollisionDataWriterCommand.IFileSystem>();
        var mockDirProvider = new Mock<CollisionDataWriterCommand.IStorageDirectoryProvider>();

        // Create a root directory path
        var rootPath = Path.GetPathRoot(Path.GetFullPath("."))!;
        var fileName = "test.log";

        mockDirProvider.Setup(p => p.GetStorageDirectory()).Returns(rootPath);

        // Mock the file system to handle the operations
        mockFileSystem.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);
        mockFileSystem.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()));

        var writer = new CollisionDataWriterCommand(
            fileName,
            new List<int[]> { new[] { 1, 2, 3 } },
            mockFileSystem.Object,
            mockDirProvider.Object
        );

        // Act
        writer.Execute();

        // Assert - Verify WriteAllLines was called
        mockFileSystem.Verify(
            f => f.WriteAllLines(
                It.Is<string>(p => p.EndsWith(fileName)),
                It.IsAny<IEnumerable<string>>()
            ),
            Times.Once
        );
    }

    private class CustomFileSystem : CollisionDataWriterCommand.IFileSystem
    {
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        public bool DirectoryExists(string path) => Directory.Exists(path);
        public void WriteAllLines(string path, IEnumerable<string> contents) =>
            File.WriteAllLines(path, contents);
    }

    private class CustomDirectoryProvider : CollisionDataWriterCommand.IStorageDirectoryProvider
    {
        private readonly string _directory;

        public CustomDirectoryProvider(string directory)
        {
            _directory = directory;
        }

        public string? GetStorageDirectory() => _directory;
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
}
