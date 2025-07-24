namespace LogParser.Common.Structs;

internal readonly record struct Checkpoint<TModel>(
    Timestamp TimeStamp,
    TModel Model
);