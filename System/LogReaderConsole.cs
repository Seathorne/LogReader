namespace LogParser.System
{
    public sealed class LogReaderConsole
    {
        public event EventHandler<LogReaderEventArgs>? OnLogRead;

        public event EventHandler<LogReaderEventArgs>? OnLogProcessed;

        public event EventHandler<LogReaderEventArgs>? OnWriteLine;

        public void WriteLine(string line)
        {
            OnWriteLine?.Invoke(null, new LogReaderEventArgs(line));
        }
    }
}
