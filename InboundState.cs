using System.Collections.Immutable;

namespace LogReader
{
    internal readonly struct InboundState(DateTimeOffset timestamp, params bool?[] statuses) : IBaseState
    {
        public DateTimeOffset TimeStamp { get; } = timestamp;

        public ImmutableArray<bool?> PrinterStatus { get; } = ImmutableArray.Create(statuses);
    }
}