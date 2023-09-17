namespace kendoku.Converters;

internal class ConvertionFailedException : Exception
{
    public ConvertionFailedException(string message) : base(message)
    {
    }
}
