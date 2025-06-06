namespace SpaceBattle;

public class ShapeRecognizerCommand : IShapeRecognizer
{
    public int GetFormId(string? formName)
    {
        if (string.IsNullOrEmpty(formName))
        {
            return 0;
        }

        return formName.ToLowerInvariant().GetHashCode();
    }
}
