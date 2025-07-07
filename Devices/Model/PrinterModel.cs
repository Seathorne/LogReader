using LogParser.Devices.Enum;

using System.Net;

namespace LogParser.Devices.Model
{
    internal record PrinterModel(
            int PrinterId,
            IPAddress PrinterIPAddress,
            DateTimeOffset? LastTimeUsed = null,
            PrinterStatus? Status = null)
        : RecordModelBase;
}