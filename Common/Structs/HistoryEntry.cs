using LogParser.Devices.Interface;

namespace LogParser.Common.Structs;

internal readonly record struct HistoryEntry<TModel>(
    Timestamp Timestamp,
    TModel ModelSnapshot
) where TModel : IDeviceModel;