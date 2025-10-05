namespace Scopist;

public sealed class ScopistValidationException : Exception
{
    public ScopistValidationException(ICollection<string> errors) : base("Scopist validation failed:\n" + string.Join("\n", errors))
    {
    }
}