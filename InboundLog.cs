using System.Text.RegularExpressions;

namespace LogReader
{
    internal class InboundLog(DateTime logTimeStamp)
    {
        public DateTime LogTimeStamp { get; } = logTimeStamp;

        private readonly IList<InboundState> inboundState = [];

        static readonly string TimeStampPattern = @"(?<time>(?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})\.(?<milliseconds>\d{3}))";
        static readonly string ThreadNumberPattern = @"(?<thread>[\d+])";
        static readonly string MessageLevelPattern = @"(?<level>INFO|DEBUG|WARNING|ERROR)";
        static readonly string EquipmentNumberPattern = @"Equipment (?<equipment>\d+)";
        static readonly string PrinterStatusPattern = @"Update Printer Receiving Line (?<line>\d) PandA (?<printer>\d) Enabled (?<status>True|False) using Tag: (?<tag>SCANNER_(?<scanner>\d+)PRINTER_W_STATUS\[(?<index>(\d+)\])";

        public void ProcessLine(string line)
        {
            var pattern = string.Format(@"{0} {1} (null) {2}\s{3} - {4} {5}",
                TimeStampPattern,
                ThreadNumberPattern,
                MessageLevelPattern,
                EquipmentNumberPattern,
                TimeStampPattern,
                PrinterStatusPattern);
            var match = Regex.Match(line, pattern);

            DateTimeOffset timestamp = new DateTimeOffset(
                year: LogTimeStamp.Year,
                month: LogTimeStamp.Month,
                day: LogTimeStamp.Day,
                hour: int.Parse(match.Groups["hour"].Value),
                minute: int.Parse(match.Groups["minute"].Value),
                second: int.Parse(match.Groups["second"].Value),
                offset: Configuration.TimeZone.GetUtcOffset(LogTimeStamp));

            // int threadNumber = int.Parse(match.Groups["thread"].Value);
            // MessageLevel messageLevel = Enum.Parse<MessageLevel>(match.Groups["level"].Value);
            // int equipmentNumber = int.Parse(match.Groups["equipment"].Value);
            int lineNumber = int.Parse(match.Groups["line"].Value);
            int printerNumber = int.Parse(match.Groups["printer"].Value);
            bool printerStatus = bool.Parse(match.Groups["status"].Value);
            // string tagName = match.Groups["tag"].Value;
            // int scannerNumber = int.Parse(match.Groups["scanner"].Value);
            // int tagIndex = int.Parse(match.Groups["index"].Value);

            bool?[] statuses = { null, null };
            statuses[printerNumber] = printerStatus;
            var state = new InboundState(timestamp, statuses);

            _ = inboundState.Append(state);
            LogReaderConsole.WriteLine(line);
            LogReaderConsole.WriteLine($"{timestamp:T} | Line {lineNumber} Printer {printerNumber} status updated: {printerStatus}" ?? "NULL");
        }
    }
}
