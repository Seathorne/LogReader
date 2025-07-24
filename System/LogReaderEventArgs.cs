namespace LogParser.System
{
    public class LogReaderEventArgs(string line)
    {
        public string Line { get; } = line;
    }
}