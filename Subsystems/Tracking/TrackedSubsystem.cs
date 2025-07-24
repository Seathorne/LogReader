using LogParser.Common.Structs;
using LogParser.Common.Tracking;
using LogParser.Subsystems.Interface;

namespace LogParser.Subsystems.Tracking;

internal abstract class TrackedSubsystem<TModel> : HistoryTracker<TModel> where TModel : ISubsystemModel
{
    public TrackedSubsystem()
    {
    }

    public TrackedSubsystem(Timestamp creationTime, TModel initialModel)
        : base(creationTime, initialModel)
    {
    }
}