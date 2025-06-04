namespace SpaceBattle.Lib.Commands;

using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CollisionDataWriterCommand : ICommand
{
    private readonly string _fileName;
    private readonly IList<int[]> _collisionPoints;

    public CollisionDataWriterCommand(string fileName, IList<int[]> collisionPoints)
    {
        _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        _collisionPoints = collisionPoints ?? throw new ArgumentNullException(nameof(collisionPoints));
    }

    public void Execute()
    {
        // Получаем из IoC директорию хранения
        var storageDir = IoC.Resolve<string>("Collision.StorageDirectory");

        if (string.IsNullOrEmpty(storageDir))
        {
            throw new InvalidOperationException("Storage directory is not set");
        }

        try
        {
            // Склеиваем полный путь
            var fullPath = Path.Combine(storageDir, _fileName);

            // Конвертируем каждый int[] в строку: числа через запятую
            var lines = _collisionPoints
                .Select(arr => string.Join(",", arr ?? Array.Empty<int>()))
                .ToList();

            // Создаём папку, если её ещё нет
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // Записываем все строки в файл
            File.WriteAllLines(fullPath, lines);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is UnauthorizedAccessException || ex is PathTooLongException)
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
}
