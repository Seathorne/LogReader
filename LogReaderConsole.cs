namespace LogReader
{
    public static class LogReaderConsole
    {
        public static event EventHandler<LogReaderEventArgs>? OnLogRead;

        public static event EventHandler<LogReaderEventArgs>? OnLogProcessed;

        public static event EventHandler<LogReaderEventArgs>? OnWriteLine;

        public static void WriteLine(string line)
        {
            OnWriteLine?.Invoke(null, new LogReaderEventArgs(line));
        }
    }
}
