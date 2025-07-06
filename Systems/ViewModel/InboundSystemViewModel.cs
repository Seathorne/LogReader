using LogParser.Systems.Model;
using LogParser.Devices.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Immutable;

namespace LogParser.Systems.ViewModel
{
    internal class InboundSystemViewModel : RecordViewModelBase<InboundSystemModel>
    {
        #region Fields
        private readonly ObservableCollection<PrinterViewModel> _printerViewModels;

        private readonly Dictionary<string, ZoneViewModel> _zoneViewModels;

        #endregion

        #region Properties

        public DateTimeOffset TimeStamp
        {
            get => Model.TimeStamp;
            set => UpdateModel(model => model with { TimeStamp = value });
        }

        public ReadOnlyObservableCollection<PrinterViewModel> Printers => new(_printerViewModels);

        public ReadOnlyDictionary<string, ZoneViewModel> Zones => new(_zoneViewModels);

        #endregion

        #region Constructors

        public InboundSystemViewModel(DateTimeOffset timeStamp,
            PrinterViewModel[] printers,
            ZoneViewModel[] zones)
            : base(new InboundSystemModel(timeStamp,
                [.. printers.Select(p => p.Model)],
                [.. zones.Select(z => z.Model)]))
        {
            _printerViewModels = new ObservableCollection<PrinterViewModel>(printers);
            _zoneViewModels = new Dictionary<string, ZoneViewModel>(zones.Length);

            foreach (var zone in zones)
            {
                _zoneViewModels.Add(zone.ZoneId, zone);
            }

            // Set up event hooks to update model when updates are made to underlying components
            foreach (var printer in printers)
            {
                printer.RequestSystemUpdate += RebuildSystemModel;
            }

            foreach (var zone in zones)
            {
                zone.RequestSystemUpdate += RebuildSystemModel;
            }
        }

        #endregion

        #region Methods

        private void RebuildSystemModel()
        {
            var currentPrinterModels = _printerViewModels.Select(vm => vm.Model).ToArray();
            UpdateModel(m => m with { Printers = ImmutableArray.Create(currentPrinterModels) });

            var currentZoneModels = _zoneViewModels.Select(entry => entry.Value.Model).ToArray();
            UpdateModel(m => m with { Zones = ImmutableArray.Create(currentZoneModels) });
        }

        #endregion
    }
}