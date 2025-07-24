using LogParser.Common.Structs;
using LogParser.Devices.Enums;
using LogParser.Devices.Interface;
using LogParser.Devices.Tracking;
using LogParser.Subsystems.Models;
using System.Collections.Immutable;
using System.Collections.Specialized;

namespace LogParser.Subsystems.Tracking
{
    internal class TrackedInboundSubsystem : TrackedSubsystem<InboundSubsystemModel>
    {
        #region Properties

        /* Each property represents an immutable collection of mutable view-models, which handle editing state. */

        public IReadOnlyDictionary<int, TrackedPrinter> Printers { get; private set; }

        public IReadOnlyDictionary<ScannerName, TrackedScanner> Scanners { get; private set; }

        #endregion

        #region Constructors

        public TrackedInboundSubsystem()
        {
            Printers = new Dictionary<int, TrackedPrinter>();
            Scanners = new Dictionary<ScannerName, TrackedScanner>();
        }

        public TrackedInboundSubsystem(
                Timestamp creationTime,
                ISet<TrackedPrinter> printers,
                ISet<TrackedScanner> scanners)
            : base(creationTime,
                  new InboundSubsystemModel(
                      printers.Select(td => td.Model).ToImmutableHashSet(),
                      scanners.Select(td => td.Model).ToImmutableHashSet()))
        {
            // Initialize underlying collections
            Printers = printers.ToImmutableDictionary(printer => printer.PrinterID);
            Scanners = scanners.ToImmutableDictionary(scanner => scanner.ScannerName);

            // Set up event hooks to update model when updates are made to devices
            foreach (var printer in printers)
            {
                printer.ModelChanged += OnTrackedDeviceChanged;
            }

            foreach (var scanner in scanners)
            {
                scanner.ModelChanged += OnTrackedDeviceChanged;
            }
        }

        #endregion

        #region Methods

        public void UpdateWithTimestamp(
            Timestamp updateTime,
            ISet<TrackedPrinter> printers,
            ISet<TrackedScanner> scanners)
        {
            // Initialize underlying collections
            Printers = printers.ToImmutableDictionary(printer => printer.PrinterID);
            Scanners = scanners.ToImmutableDictionary(scanner => scanner.ScannerName);

            // Set up event hooks to update model when updates are made to devices
            foreach (var printer in printers)
            {
                printer.ModelChanged += OnTrackedDeviceChanged;
            }

            foreach (var scanner in scanners)
            {
                scanner.ModelChanged += OnTrackedDeviceChanged;
            }

            UpdateWithTimestamp(updateTime, RebuildSystemModel());
        }

        private InboundSubsystemModel RebuildSystemModel() => new(
            Printers: [.. Printers.Select(kvp => kvp.Value.Model)],
            Scanners: [.. Scanners.Select(kvp => kvp.Value.Model)]
        );

        private void OnTrackedDeviceChanged(IDeviceModel oldModel, IDeviceModel newModel, Timestamp updateTime)
            => UpdateWithTimestamp(updateTime, RebuildSystemModel());

        private void OnTrackedSubsystemChanged(object? sender, NotifyCollectionChangedEventArgs args)
            => UpdateModel(RebuildSystemModel());

        #endregion
    }
}