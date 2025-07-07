using LogParser.Devices.Enum;

namespace LogParser.Devices.Model
{
    internal record LaneModel(int LaneNumber, LaneStatus? Status = null)
        : RecordModelBase;
}