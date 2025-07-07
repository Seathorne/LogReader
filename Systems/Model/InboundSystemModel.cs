using LogParser.Devices.Model;
using System.Collections.Immutable;

namespace LogParser.Systems.Model
{
    internal record InboundSystemModel(
            ImmutableArray<PrinterModel> Printers,
            ImmutableArray<ZoneModel> Zones,
            DateTimeOffset? TimeStamp = null)
        : SystemModelBase(TimeStamp);
}