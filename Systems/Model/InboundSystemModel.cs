using LogParser.Devices.Model;
using System.Collections.Immutable;

namespace LogParser.Systems.Model
{
    internal record InboundSystemModel(
        DateTimeOffset TimeStamp, 
        ImmutableArray<PrinterModel> Printers,
        ImmutableArray<ZoneModel> Zones);
}