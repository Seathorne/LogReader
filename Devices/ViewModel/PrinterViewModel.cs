using LogParser.Devices.Enum;
using LogParser.Devices.Model;

using System.Net;

namespace LogParser.Devices.ViewModel
{
    internal class PrinterViewModel : RecordViewModelBase<PrinterModel>
    {
        public int PrinterId => Model.PrinterId;

        public IPAddress PrinterIPAddress => Model.PrinterIPAddress;

        public PrinterStatus? Status
        {
            get => Model.Status;
            set => UpdateModel(printer => printer with { Status = value });
        }

        public DateTimeOffset? LastTimeUsed
        {
            get => Model.LastTimeUsed;
            set => UpdateModel(printer => printer with { LastTimeUsed = value });
        }

        public PrinterViewModel(int id, IPAddress ipAddress, DateTimeOffset? lastTimeUsed = null, PrinterStatus? status = null)
            : base(new PrinterModel(id, ipAddress, lastTimeUsed, status))
        {
            Model = new PrinterModel(id, ipAddress, lastTimeUsed, status);
        }
    }
}