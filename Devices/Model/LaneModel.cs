using LogParser.Devices.Enum;

namespace LogParser.Devices.Model
{
    internal record LaneModel(int? LaneID = null, LaneStatus? Status = null)
        : RecordModelBase
    {
        public LaneModel() : this(null, null)
        {
        }
    }
}