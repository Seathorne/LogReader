using LogParser.LogState.Inbound;

namespace LogParser
{
    public class LogReader : IDisposable
    {
        private FileStream? fileStream;
        private StreamReader? streamReader;

        private InboundLog? inboundLog;

        public required TimeZoneInfo TimeZone { get; set; }

        public LogReaderConsole Console { get; } = new();

        public void Dispose()
        {
            fileStream?.Dispose();
            streamReader?.Dispose();
            GC.SuppressFinalize(this);

            fileStream = null;
            streamReader = null;
        }

        public void ProcessFile(string filePath)
        {
            fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            streamReader = new StreamReader(fileStream);
            inboundLog = new InboundLog(File.GetLastWriteTime(filePath), TimeZone, Console);

            string[] lines = streamReader.ReadToEnd().Split(Environment.NewLine);
            foreach (string line in lines)
            {
                inboundLog.ProcessLine(line);
            }
        }
    }
}
