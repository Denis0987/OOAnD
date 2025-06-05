namespace SpaceBattle.Lib.Commands;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hwdtech;

public class CollisionDataWriterCommand : ICommand
{
    private readonly string _fileName;
    private readonly IList<int[]> _collisionPoints;
    private readonly IFileSystem _fileSystem;
    private readonly IStorageDirectoryProvider _directoryProvider;

    public CollisionDataWriterCommand(
        string fileName,
        IList<int[]> collisionPoints,
        IFileSystem? fileSystem = null,
        IStorageDirectoryProvider? directoryProvider = null)
    {
        _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        _collisionPoints = collisionPoints ?? throw new ArgumentNullException(nameof(collisionPoints));
        _fileSystem = fileSystem ?? new DefaultFileSystem();
        _directoryProvider = directoryProvider ?? new DefaultStorageDirectoryProvider();

        // Get just the file name part for validation
        var fileNameOnly = Path.GetFileName(_fileName);

        // Check if the file name is null, empty, or whitespace
        if (string.IsNullOrWhiteSpace(fileNameOnly))
        {
            throw new ArgumentException("File name cannot be empty or whitespace", nameof(fileName));
        }

        // Check for invalid file name characters
        var invalidChars = Path.GetInvalidFileNameChars();
        if (fileNameOnly.IndexOfAny(invalidChars) >= 0)
        {
            throw new ArgumentException("File name contains invalid characters", nameof(fileName));
        }
    }

    public void Execute()
    {
        var storageDir = _directoryProvider.GetStorageDirectory();

        if (string.IsNullOrWhiteSpace(storageDir))
        {
            throw new InvalidOperationException("Storage directory is not set");
        }

        try
        {
            var fullPath = Path.Combine(storageDir, _fileName);
            var lines = _collisionPoints
                .Select(arr => string.Join(",", arr ?? Array.Empty<int>()))
                .ToList();

            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !_fileSystem.DirectoryExists(dir))
            {
                _fileSystem.CreateDirectory(dir);
            }

            _fileSystem.WriteAllLines(fullPath, lines);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or UnauthorizedAccessException or PathTooLongException)
        {
            throw new InvalidOperationException("Invalid file path or access denied", ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new InvalidOperationException("Directory not found", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Error writing to file", ex);
        }
    }

    public interface IFileSystem
    {
        void CreateDirectory(string path);
        void WriteAllLines(string path, IEnumerable<string> contents);
        bool DirectoryExists(string path);
    }

    public interface IStorageDirectoryProvider
    {
        string? GetStorageDirectory();
    }

    private class DefaultFileSystem : IFileSystem
    {
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        public void WriteAllLines(string path, IEnumerable<string> contents) => File.WriteAllLines(path, contents);
        public bool DirectoryExists(string path) => Directory.Exists(path);
    }

    private class DefaultStorageDirectoryProvider : IStorageDirectoryProvider
    {
        public string? GetStorageDirectory() => IoC.Resolve<string>("Collision.StorageDirectory");
    }
}
