using LogParser.Messages;

namespace LogParser
{
    internal class InboundLog(DateTime logTimeStamp, TimeZoneInfo timeZone, LogReaderConsole console)
    {
        private readonly LogReaderConsole console = console;

        public DateTime LogTimeStamp { get; } = logTimeStamp;

        private readonly IList<InboundState> inboundStates = [];

        public void ProcessLine(string line)
        {
            bool parsed = PrinterStatusUpdateMessage.TryParse(line, LogTimeStamp, timeZone, out var result);

            if (!parsed) return;

            InboundState state = result?.PrinterNumber switch
            {
                1 or 3 => new InboundState(result.EventTimeStamp, result.PrinterStatus, inboundStates.LastOrDefault().Printer2Status),
                2 or 4 => new InboundState(result.EventTimeStamp, inboundStates.LastOrDefault().Printer1Status, result.PrinterStatus),
                _ => throw new IndexOutOfRangeException($"Printer number \"{result?.PrinterNumber}\" is out of range. Acceptable values are [1, 2, 3, 4].")
            };
            _ = inboundStates.Append(state);

            console.WriteLine($"{state.TimeStamp:T} | {result.PrinterName} status updated: {result.PrinterStatus}");
        }
    }
}
