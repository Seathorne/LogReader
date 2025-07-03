using LogParser.Messages;

using System.Collections.Immutable;
using System.Text;

namespace LogParser.LogState.Inbound
{
    internal class InboundLog(DateTime logTimeStamp, TimeZoneInfo timeZone, LogReaderConsole console)
    {
        private readonly LogReaderConsole console = console;

        public DateTime LogTimeStamp { get; } = logTimeStamp;

        private readonly IList<InboundState> inboundStates = [];

        public void ProcessLine(string line)
        {
            bool parsed = false;
            bool?[] printerStatuses = inboundStates.Count >= 1 ? [.. inboundStates.Last().PrinterStatuses] : [];
            LaneStatus[] laneStatuses = inboundStates.Count >= 1 ? [.. inboundStates.Last().LaneStatuses] : [];

            parsed |= PrinterStatusUpdateMessage.TryParse(line, LogTimeStamp, timeZone, out var printerStatusUpdate);
            if (parsed)
            {
                // update printer status
                printerStatuses = new bool?[printerStatusUpdate.PrinterNumber];
                printerStatuses[printerStatusUpdate.PrinterNumber - 1] = printerStatusUpdate.PrinterStatus;

                // update state
                InboundState inboundState = new(printerStatusUpdate.EventTimeStamp, ImmutableArray.Create(printerStatuses), ImmutableArray.Create(laneStatuses));
                inboundStates.Add(inboundState);

                // update console
                console.WriteLine($"{inboundState.TimeStamp:HH:mm:ss.fff} {printerStatusUpdate.PrinterName} status updated: {printerStatusUpdate.PrinterStatus}");

                return;
            }

            parsed |= LaneStatusUpdateMessage.TryParse(line, LogTimeStamp, timeZone, out var laneStatusUpdate);
            if (parsed)
            {
                // update lane status
                int newLaneCount = laneStatusUpdate.LaneStatuses.Length;
                laneStatuses = newLaneCount > laneStatuses.Length ? new LaneStatus[newLaneCount] : laneStatuses;
                foreach (var lane in laneStatusUpdate.LaneStatuses.Where(laneStatusPair => laneStatusPair.LaneNumber != 0))
                {
                    laneStatuses[lane.LaneNumber - 1] = lane.LaneStatus;
                }

                // update state
                InboundState inboundState = new(laneStatusUpdate.TimeStamp, ImmutableArray.Create(printerStatuses), ImmutableArray.Create(laneStatuses));
                inboundStates.Add(inboundState);

                // update console
                string laneStatusesString = string.Join(", ", laneStatuses.Select((status, index) => $"{index}: {status}"));
                console.WriteLine($"{inboundState.TimeStamp:HH:mm:ss.fff} Lane statuses updated: [{laneStatusesString}]");

                return;
            }

            return; // TODO: throw exception for unknown message types
        }
    }
}
