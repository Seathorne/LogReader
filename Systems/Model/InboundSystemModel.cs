using LogParser.Devices.Enum;
using LogParser.Devices.Model;
using LogParser.Devices.ViewModel;
using System.Collections.Immutable;

namespace LogParser.Systems.Model
{
    internal record InboundSystemModel(
            ImmutableArray<PrinterModel> Printers,
            ImmutableArray<ZoneModel> Zones,
            ILookup<ScannerName, ContainerViewModel> QueuedUpContainers,
            DateTimeOffset? TimeStamp = null)
        : SystemModelBase(TimeStamp);
}