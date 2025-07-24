using LogParser.Common.Structs;
using LogParser.Common.Tracking;
using LogParser.Devices.Interface;

namespace LogParser.Devices.Tracking;

internal class TrackedDevice<TModel> : HistoryTracker<TModel> where TModel : IDeviceModel
{
    public TrackedDevice()
    {
    }

    public TrackedDevice(Timestamp creationTime, TModel initialModel)
        : base(creationTime, initialModel)
    {
    }
}