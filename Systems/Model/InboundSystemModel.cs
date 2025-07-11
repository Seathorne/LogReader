using LogParser.Devices.Enum;
using LogParser.Devices.Model;
using System.Collections.Immutable;

namespace LogParser.Systems.Model
{
    internal record InboundSystemModel : SystemModelBase
    {
        #region Properties

        public ImmutableArray<PrinterModel> Printers { get; init; }

        public ImmutableArray<ZoneModel> Zones { get; init; }

        public ImmutableArray<(ScannerName ScannerID, ImmutableArray<ContainerModel> Containers)> QueuedContainers { get; init; }

        #endregion

        #region Constructors

        public InboundSystemModel()
        {
            Printers = [];
            Zones = [];
            QueuedContainers = [];
        }

        public InboundSystemModel(
            ISet<PrinterModel> printers,
            ISet<ZoneModel> zones,
            IDictionary<ScannerName, Queue<ContainerModel>> queuedContainers)
        {
            Printers = [.. printers];
            Zones = [.. zones];
            QueuedContainers = [.. queuedContainers.Select(kvp => (kvp.Key, kvp.Value.ToImmutableArray()))];
        }

        public InboundSystemModel(
            ImmutableArray<PrinterModel> printers,
            ImmutableArray<ZoneModel> zones,
            ImmutableArray<(ScannerName ScannerID, ImmutableArray<ContainerModel> Containers)> queuedContainers)
        {
            Printers = printers;
            Zones = zones;
            QueuedContainers = queuedContainers;
        }

        #endregion
    }
}