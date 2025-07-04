using LogParser.Devices.Enum;

namespace LogParser.Devices.Model
{
    internal record PrinterModel(int PrinterId, PrinterStatus? Status = null);
}