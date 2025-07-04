using LogParser.Systems.Model;
using LogParser.Devices.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Immutable;

namespace LogParser.Systems.ViewModel
{
    internal class InboundSystemViewModel : RecordViewModelBase<InboundSystemModel>
    {
        private readonly ObservableCollection<PrinterViewModel> _printerViewModels;

        public DateTimeOffset TimeStamp => Model.TimeStamp;

        public ReadOnlyObservableCollection<PrinterViewModel> Printers => new(_printerViewModels);

        public InboundSystemViewModel(DateTimeOffset timeStamp, params PrinterViewModel[] printers)
            : base(new InboundSystemModel(timeStamp, [.. printers.Select(p => p.Model)]))
        {
            _printerViewModels = new ObservableCollection<PrinterViewModel>(printers);

            // Initialize view-model lookup dictionary
            foreach (var printer in printers)
            {
                printer.RequestSystemUpdate += RebuildSystemModel;
            }
        }

        private void RebuildSystemModel()
        {
            var currentPrinterModels = _printerViewModels.Select(vm => vm.Model).ToArray();
            UpdateModel(m => m with { Printers = ImmutableArray.Create(currentPrinterModels) });
        }
    }
}