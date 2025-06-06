namespace SpaceBattle.Tests;

public class ShapeRecognizerCommandTests
{
    [Fact]
    public void GetFormId_ValidForm_ReturnsHashCode()
    {
        var formRecognizer = new ShapeRecognizerCommand();
        var formName = "rectangle";

        var expectedHash = "rectangle".ToLowerInvariant().GetHashCode();

        var formId = formRecognizer.GetFormId(formName);

        Assert.Equal(expectedHash, formId);
    }

    [Fact]
    public void GetFormId_EmptyForm_ReturnsZero()
    {
        var formRecognizer = new ShapeRecognizerCommand();

        var formId = formRecognizer.GetFormId(string.Empty);

        Assert.Equal(0, formId);
    }

    [Fact]
    public void GetFormId_NullForm_ReturnsZero()
    {
        var formRecognizer = new ShapeRecognizerCommand();

        var formId = formRecognizer.GetFormId(null);

        Assert.Equal(0, formId);
    }

    [Fact]
    public void GetFormId_SameForm_ReturnsConsistentHashCode()
    {
        var formRecognizer = new ShapeRecognizerCommand();
        var formName1 = "pentagon";
        var formName2 = "pentagon";

        var formId1 = formRecognizer.GetFormId(formName1);
        var formId2 = formRecognizer.GetFormId(formName2);

        Assert.Equal(formId1, formId2);
    }

    [Fact]
    public void GetFormId_IgnoreCase_ReturnsConsistentHashCode()
    {
        var formRecognizer = new ShapeRecognizerCommand();
        var formName1 = "hexagon";
        var formName2 = "HEXAGON";

        var formId1 = formRecognizer.GetFormId(formName1);
        var formId2 = formRecognizer.GetFormId(formName2);

        Assert.Equal(formId1, formId2);
    }
}
