namespace LogParser.Common.Exceptions;

public class UninitializedException : InvalidOperationException
{
    public string? ObjectName { get; }

    public UninitializedException(string objectName)
        : base($"The object \"{objectName}\" has not been initialized.")
    {
        ObjectName = objectName;
    }

    public UninitializedException(string objectName, string message)
        : base(message)
    {
        ObjectName = objectName;
    }
}