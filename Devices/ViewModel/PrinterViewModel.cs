using LogParser.Devices.Enum;
using LogParser.Devices.Model;

namespace LogParser.Devices.ViewModel
{
    internal class PrinterViewModel : RecordViewModelBase<PrinterModel>
    {
        public int PrinterId => Model.PrinterId;

        public PrinterStatus? Status
        {
            get => Model.Status;
            set => UpdateModel(printer => printer with { Status = value });
        }

        public PrinterViewModel(int id, PrinterStatus? status = null) : base(new PrinterModel(id, status))
        {
            Model = new PrinterModel(id, status);
        }
    }
}