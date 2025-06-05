namespace SpaceBattle.Lib.Tests.CommandTests;

using System;
using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;
using Xunit;

public class IoCRegisterCollisionFileNameFormatterCommandTests : IDisposable
{
    private readonly object _rootScope;
    private bool _disposed;
    private readonly object _testScope;

    public IoCRegisterCollisionFileNameFormatterCommandTests()
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
    public void Execute_ShouldRegisterFormatterStrategy()
    {
        var command = new IoCRegisterCollisionFileNameFormatterCommand();

        command.Execute();

        var result = IoC.Resolve<string>("Collision.FileNameFormatter", "One", "Two");
        Assert.Equal("One__Two.log", result);
    }

    [Fact]
    public void Formatter_WithNullFirstArgument_ShouldThrowArgumentException()
    {
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        var exception = Assert.Throws<ArgumentException>(() => IoC.Resolve<string>("Collision.FileNameFormatter", null!, "valid"));

        Assert.Contains("First argument cannot be null", exception.Message);
    }
    [Fact]
    public void IoCRegisterCollisionFileNameFormatterCommand_NullInput_TriggersErrorPath()
    {
        // Arrange
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Hwdtech.IoC.Resolve<string>("Collision.FileNameFormatter", null!, "valid"));
        
        Assert.Contains("First argument cannot be null", exception.Message);
    }
    [Fact]
    public void Formatter_WithNullSecondArgument_ShouldThrowArgumentException()
    {
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        var exception = Assert.Throws<ArgumentException>(() => IoC.Resolve<string>("Collision.FileNameFormatter", "valid", null!));

        Assert.Contains("Second argument cannot be null", exception.Message);
    }

    [Fact]
    public void Formatter_WithEmptyStrings_ShouldFormatCorrectly()
    {
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        var result = IoC.Resolve<string>("Collision.FileNameFormatter", string.Empty, string.Empty);

        Assert.Equal("__.log", result);
    }

    [Fact]
    public void Formatter_WithSpecialCharacters_ShouldFormatCorrectly()
    {
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        var result = IoC.Resolve<string>("Collision.FileNameFormatter", "File@123", "Name#456");

        Assert.Equal("File@123__Name#456.log", result);
    }

    [Fact]
    public void Formatter_WithNullArgs_ShouldThrowException()
    {
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        var exception = Assert.Throws<ArgumentException>(() => IoC.Resolve<string>("Collision.FileNameFormatter", null!, null!));
        Assert.Contains("First argument cannot be null", exception.Message);
    }

    [Fact]
    public void Formatter_WithInsufficientArguments_ShouldThrowException()
    {
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        var exception = Assert.Throws<ArgumentException>(() => IoC.Resolve<string>("Collision.FileNameFormatter", "onlyOne"));
        Assert.Contains("Two arguments are required", exception.Message);
    }

    [Theory]
    [InlineData("file1", "name1", "file1__name1.log")]
    [InlineData("file2", "name2", "file2__name2.log")]
    [InlineData("file3", "name3", "file3__name3.log")]
    public void Formatter_WithDifferentInputs_ShouldFormatCorrectly(string first, string second, string expected)
    {
        // Arrange
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        // Act
        var result = IoC.Resolve<string>("Collision.FileNameFormatter", first, second);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Formatter_WithVeryLongStrings_ShouldFormatCorrectly()
    {
        // Arrange
        var longString = new string('a', 1000);
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        // Act
        var result = IoC.Resolve<string>("Collision.FileNameFormatter", longString, longString);

        // Assert
        Assert.Equal($"{longString}__{longString}.log", result);
    }

    [Fact]
    public void Formatter_WithSpecialUnicodeCharacters_ShouldFormatCorrectly()
    {
        // Arrange
        var unicodeStr = "こんにちは世界";
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        // Act
        var result = IoC.Resolve<string>("Collision.FileNameFormatter", unicodeStr, unicodeStr);

        // Assert
        Assert.Equal($"{unicodeStr}__{unicodeStr}.log", result);
    }
}
