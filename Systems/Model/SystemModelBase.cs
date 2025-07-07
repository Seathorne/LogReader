using LogParser.Devices.Model;

namespace LogParser.Systems.Model
{
    internal record SystemModelBase(DateTimeOffset? TimeStamp = null) : RecordModelBase;
}