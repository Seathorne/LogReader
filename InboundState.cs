using System.Collections.Immutable;

namespace LogParser
{
    internal readonly struct InboundState(DateTimeOffset timestamp, bool? printer1Status = null, bool? printer2Status = null) : IBaseState
    {
        public DateTimeOffset TimeStamp { get; } = timestamp;

        public ImmutableArray<bool?> PrinterStatus => [Printer1Status, Printer2Status];

        public bool? Printer1Status { get; } = printer1Status;

        public bool? Printer2Status { get; } = printer2Status;
    }
}