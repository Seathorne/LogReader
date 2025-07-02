using LogReader.Messages;

namespace LogReader
{
    internal class InboundLog(DateTime logTimeStamp)
    {
        public DateTime LogTimeStamp { get; } = logTimeStamp;

        private readonly IList<InboundState> inboundStates = [];

        public void ProcessLine(string line)
        {
            bool parsed = PrinterStatusUpdateMessage.TryParse(line, LogTimeStamp, out var result);

            if (!parsed) return;

            InboundState state = result.PrinterNumber switch
            {
                1 => new InboundState(result.EventTimeStamp, result.PrinterStatus, inboundStates.LastOrDefault().Printer2Status),
                2 => new InboundState(result.EventTimeStamp, inboundStates.LastOrDefault().Printer1Status, result.PrinterStatus),
                _ => throw new IndexOutOfRangeException($"Printer number \"{result.PrinterNumber}\" is out of range. Acceptable values are [1, 2].")
            };
            _ = inboundStates.Append(state);

            LogReaderConsole.WriteLine(line);
            LogReaderConsole.WriteLine($"{state.TimeStamp:T} | {result.PrinterName} status updated: {result.PrinterStatus}");
        }
    }
}
