namespace LogParser.Equipment.Common.Exceptions;

public class UnknownMessageException : InvalidOperationException
{
    public string? MessageLine { get; }

    public UnknownMessageException(string messageLine)
        : base($"The message was unable to be parsed: {messageLine}")
    {
        MessageLine = messageLine;
    }

    public UnknownMessageException(string messageLine, string message)
        : base(message)
    {
        MessageLine = messageLine;
    }
}