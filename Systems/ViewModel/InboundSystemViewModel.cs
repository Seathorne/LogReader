using LogParser.Devices.Enum;
using LogParser.Devices.ViewModel;
using LogParser.Systems.Model;
using LogParser.Utility;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace LogParser.Systems.ViewModel
{
    internal class InboundSystemViewModel : SystemViewModelBase<InboundSystemModel>
    {
        #region Fields

        private readonly ObservableDictionary<int, PrinterViewModel> _printerViewModels;

        private readonly ObservableDictionary<string, ZoneViewModel> _zoneViewModels;

        private readonly ObservableDictionary<ScannerName, ObservableCollection<ContainerViewModel>> _queuedUpContainers;

        #endregion

        #region Properties

        /* Each property represents an immutable collection of mutable view-models, which handle editing state. */

        public ReadOnlyDictionary<int, PrinterViewModel> Printers => new(_printerViewModels);

        public ReadOnlyDictionary<string, ZoneViewModel> Zones => new(_zoneViewModels);

        public ILookup<ScannerName, ContainerViewModel> QueuedContainersLookup => DictionaryExtensions.ToLookup(_queuedUpContainers);

        public ReadOnlyDictionary<ScannerName, ObservableCollection<ContainerViewModel>> QueuedUpContainers => new(_queuedUpContainers);

        #endregion

        #region Constructors

        public InboundSystemViewModel(
                PrinterViewModel[] printers,
                ZoneViewModel[] zones,
                (ScannerName ScannerName, List<ContainerViewModel> Containers)[] queuedContainers)
            : base(new InboundSystemModel(
                Printers: [.. printers.Select(p => p.Model)],
                Zones: [.. zones.Select(z => z.Model)],
                QueuedUpContainers: queuedContainers.ToDictionary().ToLookup()))
        {
            // Initialize underlying collections
            _printerViewModels = new ObservableDictionary<int, PrinterViewModel>(printers.Select(p => (p.PrinterId, p)).ToDictionary());
            _zoneViewModels = new ObservableDictionary<string, ZoneViewModel>(zones.Select(z => (z.ZoneId, z)).ToDictionary());
            _queuedUpContainers = 
                queuedContainers.Select(scanner => 
                    (scanner.ScannerName, new ObservableCollection<ContainerViewModel>(scanner.Containers)))
                .ToDictionary().ToObservableDictionary();

            // Set up event hooks to update model when updates are made to underlying collections
            _printerViewModels.ItemAdded += (sender, args) => RebuildSystemModel();
            _printerViewModels.ItemRemoved += (sender, args) => RebuildSystemModel();
            _zoneViewModels.ItemAdded += (zoneId, zoneVm) => RebuildSystemModel();
            _zoneViewModels.ItemRemoved += (zoneId, zoneVm) => RebuildSystemModel();

            var scanners = Enum.GetValues<ScannerName>();
            foreach (var scanner in scanners.Where(name => name != ScannerName.None))
            {
                _queuedUpContainers.ItemAdded += (scanner, queuedContainers) => RebuildSystemModel();
                _queuedUpContainers.ItemRemoved += (scanner, queuedContainers) => RebuildSystemModel();
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

            foreach (var queuedUpContainers in queuedContainers.Select(kvp => kvp.Containers))
            {
                foreach (var container in queuedUpContainers)
                {
                    container.RequestSystemUpdate += RebuildSystemModel;
                }
            }
        }

        #endregion

        #region Methods

        private void RebuildSystemModel()
        {
            var currentPrinterModels = _printerViewModels.Select(entry => entry.Value.Model).ToArray();
            var currentZoneModels = _zoneViewModels.Select(entry => entry.Value.Model).ToArray();

            UpdateModel(m => m with
            {
                Printers = ImmutableArray.Create(currentPrinterModels),
                Zones = ImmutableArray.Create(currentZoneModels),
                QueuedUpContainers = QueuedContainersLookup
            });
        }

        #endregion
    }
}