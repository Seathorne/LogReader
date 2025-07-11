using LogParser.Devices.Enum;
using LogParser.Devices.Model;
using LogParser.Devices.ViewModel;
using LogParser.Systems.Model;
using LogParser.Utility;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LogParser.Systems.ViewModel
{
    internal class InboundSystemViewModel : SystemViewModelBase<InboundSystemModel>
    {
        #region Properties

        /* Each property represents an immutable collection of mutable view-models, which handle editing state. */

        public ObservableDictionary<int, PrinterViewModel> Printers { get; }

        public ObservableDictionary<string, ZoneViewModel> Zones { get; }

        public ObservableDictionary<ScannerName, ObservableCollection<ContainerViewModel>> QueuedContainers { get; }

        #endregion

        #region Constructors

        public InboundSystemViewModel()
        {
            Printers = [];
            Zones = [];
            QueuedContainers = [];
        }

        public InboundSystemViewModel(
                ISet<PrinterViewModel> printers,
                ISet<ZoneViewModel> zones,
                IDictionary<ScannerName, Queue<ContainerViewModel>> queuedContainers)
            : base(
                new InboundSystemModel(
                    printers: new HashSet<PrinterModel>(printers.Select(vm => vm.Model)),
                    zones: new HashSet<ZoneModel>(zones.Select(vm => vm.Model)),
                    queuedContainers: queuedContainers.ToDictionary(kvp => kvp.Key, kvp => new Queue<ContainerModel>(kvp.Value.Select(vm => vm.Model)))
                ))
        {
            // Initialize underlying collections
            Printers = printers.ToObservableDictionary(vm => vm.PrinterID ?? throw new NullReferenceException(), vm => vm);
            Zones = zones.ToObservableDictionary(vm => vm.ZoneId ?? throw new NullReferenceException(), vm => vm);
            QueuedContainers = queuedContainers.ToObservableDictionary(kvp => kvp.Key, kvp => new ObservableCollection<ContainerViewModel>(kvp.Value));

            // Set up event hooks to update model when updates are made to device collections
            Printers.CollectionChanged += OnSystemViewModelChanged;
            Zones.CollectionChanged += OnSystemViewModelChanged;
            QueuedContainers.CollectionChanged += OnSystemViewModelChanged;

            var scanners = Enum.GetValues<ScannerName>();
            foreach (var scanner in scanners.Where(name => name != ScannerName.None))
            {
                QueuedContainers[scanner].CollectionChanged += OnSystemViewModelChanged;
            }

            // Set up event hooks to update model when updates are made to devices
            foreach (var printer in printers)
            {
                printer.ModelChanged += OnDeviceViewModelChanged;
            }

            foreach (var zone in zones)
            {
                zone.ModelChanged += OnDeviceViewModelChanged;
            }

            foreach (var queue in queuedContainers.Values)
            {
                foreach (var container in queue)
                {
                    container.ModelChanged += OnDeviceViewModelChanged;
                }
            }
        }

        #endregion

        #region Methods

        private InboundSystemModel RebuildSystemModel() => new(
            printers: [.. Printers.Select(kvp => kvp.Value.Model)],
            zones: [.. Zones.Select(kvp => kvp.Value.Model)],
            queuedContainers: [.. QueuedContainers.Select(kvp => (kvp.Key, kvp.Value.Select(vm => vm.Model).ToImmutableArray()))]
        );

        private void OnDeviceViewModelChanged(RecordModelBase oldModel, RecordModelBase newModel)
            => UpdateModel(RebuildSystemModel());

        private void OnSystemViewModelChanged(object? sender, NotifyCollectionChangedEventArgs args)
            => UpdateModel(RebuildSystemModel());

        #endregion
    }
}