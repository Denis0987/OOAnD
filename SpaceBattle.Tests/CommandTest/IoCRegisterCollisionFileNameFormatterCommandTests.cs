namespace SpaceBattle.Lib.Tests.CommandTests;

using System;
using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib.Commands;
using Xunit;
using System.Collections.Generic;

public class IoCRegisterCollisionFileNameFormatterCommandTests : IDisposable
{
    private readonly object _rootScope;
    private bool _disposed;
    private readonly object _testScope;

    public IoCRegisterCollisionFileNameFormatterCommandTests()
    {
        try
        {
            // Initialize the IoC container implementation
            new InitScopeBasedIoCImplementationCommand().Execute();
            
            // Get the root scope
            _rootScope = IoC.Resolve<object>("Scopes.Root");

            // Create a new scope for the test
            _testScope = IoC.Resolve<object>("Scopes.New", _rootScope);
            IoC.Resolve<ICommand>("Scopes.Current.Set", _testScope).Execute();

            // Register required dependencies
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "IoC.Scope.Current",
                (object[] _) => _testScope
            ).Execute();

            // Register a simple storage directory for testing
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
            // Reset the IoC container to root scope
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
        // Arrange
        var command = new IoCRegisterCollisionFileNameFormatterCommand();

        // Act: Register the formatter
        command.Execute();

        // Assert: Check that the formatter is registered and works correctly
        var result = IoC.Resolve<string>("Collision.FileNameFormatter", "One", "Two");
        Assert.Equal("One__Two.log", result);
    }

    [Fact]
    public void Formatter_WithNullFirstArgument_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => IoC.Resolve<string>("Collision.FileNameFormatter", null!, "valid"));
        
        Assert.Contains("First argument cannot be null", exception.Message);
    }
    
    [Fact]
    public void Formatter_WithNullSecondArgument_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => IoC.Resolve<string>("Collision.FileNameFormatter", "valid", null!));
        
        Assert.Contains("Second argument cannot be null", exception.Message);
    }
    




    [Fact]
    public void Formatter_WithEmptyStrings_ShouldFormatCorrectly()
    {
        // Arrange
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        // Act
        var result = IoC.Resolve<string>("Collision.FileNameFormatter", string.Empty, string.Empty);

        // Assert
        Assert.Equal("__.log", result);
    }

    [Fact]
    public void Formatter_WithSpecialCharacters_ShouldFormatCorrectly()
    {
        // Arrange
        var command = new IoCRegisterCollisionFileNameFormatterCommand();
        command.Execute();

        // Act
        var result = IoC.Resolve<string>("Collision.FileNameFormatter", "File@123", "Name#456");
        
        // Assert
        Assert.Equal("File@123__Name#456.log", result);
    }
}
