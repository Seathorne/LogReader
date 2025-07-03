using System.Collections.Immutable;

namespace LogParser.LogState.Inbound
{
    internal record InboundState : IStateBase
    {
        public InboundState(DateTimeOffset timeStamp, ImmutableArray<bool?> printerStatuses, ImmutableArray<LaneStatus> laneStatuses)
        {
            TimeStamp = timeStamp;
            PrinterStatuses = printerStatuses;
            LaneStatuses = laneStatuses;
        }

        public DateTimeOffset TimeStamp { get; }

        public ImmutableArray<bool?> PrinterStatuses { get; }

        public ImmutableArray<LaneStatus> LaneStatuses { get; }
        
    }
}