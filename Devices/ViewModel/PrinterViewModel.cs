using LogParser.Devices.Enum;
using LogParser.Devices.Model;

using System.Net;

namespace LogParser.Devices.ViewModel
{
    internal class PrinterViewModel : RecordViewModelBase<PrinterModel>
    {
        #region Properties

        public int? PrinterID
        {
            get => Model.PrinterID;
            set => UpdateModel(m => m with { PrinterID = value });
        }

        public PrinterStatus? Status
        {
            get => Model.Status;
            set => UpdateModel(m => m with { Status = value });
        }

        public TimeOnly? LastUsedTime
        {
            get => Model.LastUsedTime;
            set => UpdateModel(m => m with { LastUsedTime = value });
        }

        public IPAddress? IPAddress
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(IPAddress));
            }
        }

        #endregion

        #region Constructors

        public PrinterViewModel()
        {
        }

        public PrinterViewModel(int printerID, IPAddress ipAddress) : base(new PrinterModel(printerID))
        {
            IPAddress = ipAddress;
        }

        public PrinterViewModel(int printerID, IPAddress ipAddress, PrinterStatus? status = null, TimeOnly? lastUsedTime = null)
            : base(new PrinterModel(printerID, status, lastUsedTime))
        {
            IPAddress = ipAddress;
        }

        #endregion
    }
}