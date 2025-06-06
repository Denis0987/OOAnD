
using Hwdtech.Ioc;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyShapeRecognizerCommandTests
{
    public RegisterIoCDependencyShapeRecognizerCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void RegisterFormRecognizer_Succeeds()
    {
        new RegisterIoCDependencyCollisionShapeCommand().Execute();

        var formRecognizer = IoC.Resolve<IShapeRecognizer>("Collision.FormRecognizer");
        Assert.NotNull(formRecognizer);

        var formId = formRecognizer.GetFormId("rectangle");
        Assert.Equal("rectangle".ToLowerInvariant().GetHashCode(), formId);
    }
}
