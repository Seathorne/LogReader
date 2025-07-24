namespace LogParser.Common.Structs;

internal readonly record struct HistoryDelta(
    Timestamp Timestamp,
    IReadOnlyDictionary<string, object?> ChangedProperties
);