namespace SpaceBattle.Lib.Tests.CommandTests;

using SpaceBattle.Lib.Commands;
using Xunit;

public class IoCRegisterCollisionFileNameFormatterCommandTests
{
    public IoCRegisterCollisionFileNameFormatterCommandTests()
    {
        // Инициализируем IoC контейнер
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void Execute_ShouldRegisterFormatterStrategy()
    {
        // Act: вызываем регистрацию
        new IoCRegisterCollisionFileNameFormatterCommand().Execute();

        // Assert: IoC должен возвращать корректную стратегию
        var result = IoC.Resolve<string>(
            "Collision.FileNameFormatter",
            "One",
            "Two"
        );

        // Проверяем форматирование
        Assert.Equal("One__Two.log", result);
    }
}
