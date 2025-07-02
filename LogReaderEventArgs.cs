namespace LogParser
{
    public class LogReaderEventArgs(string line)
    {
        public string Line { get; } = line;
    }
}