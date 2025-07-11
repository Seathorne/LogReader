using LogParser.Devices.Enum;

namespace LogParser.Devices.Model
{
    internal record PrinterModel(
            int? PrinterID = null,
            PrinterStatus? Status = null,
            TimeOnly? LastUsedTime = null)
        : RecordModelBase
    {
        public PrinterModel() : this(null, null, null)
        {
        }
    }
}