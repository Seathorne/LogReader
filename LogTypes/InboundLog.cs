using LogParser.Devices.Enum;
using LogParser.Devices.ViewModel;
using LogParser.Messages;
using LogParser.Systems.Model;
using LogParser.Systems.ViewModel;

using System.Net;

namespace LogParser.LogTypes
{
    internal class InboundLog(DateTimeOffset logTimeStamp, LogReaderConsole console)
    {
        #region Fields

        private readonly LogReaderConsole _console = console;

        private readonly IList<InboundSystemModel> inboundHistory = [];

        #endregion

        #region Properties

        public DateTimeOffset LogTimeStamp { get; } = logTimeStamp;

        public InboundSystemViewModel ViewModel { get; } = new(timeStamp: logTimeStamp,
            printers: [
                new PrinterViewModel(id: 1, ipAddress: new IPAddress([172, 24, 18, 38])),
                new PrinterViewModel(id: 2, ipAddress: new IPAddress([172, 24, 18, 39])),
                new PrinterViewModel(id: 3, ipAddress: new IPAddress([172, 24, 18, 40])),
                new PrinterViewModel(id: 4, ipAddress: new IPAddress([172, 24, 18, 41]))
            ],
            zones: [
                new ZoneViewModel(id: "000")
            ]);

        #endregion

        #region Methods

        public void ProcessLine(string line)
        {
            bool parsed = false;
            PrinterStatus?[] printerStatuses = inboundHistory.Count >= 1
                ? [.. inboundHistory.Last().Printers.Select(p => p.Status)]
                : [];

            parsed |= PrinterStatusUpdateMessage.TryParse(line, LogTimeStamp, out var printerStatusUpdate);
            if (parsed)
            {
                int index = printerStatusUpdate.ConveyorLineNumber - 1;

                // update state
                ViewModel.Printers[index].Status = printerStatusUpdate.PrinterStatus;
                ViewModel.TimeStamp = printerStatusUpdate.EventTimeStamp;

                // store history
                inboundHistory.Add(ViewModel.Model);

                // update console
                _console.WriteLine($"{ViewModel.TimeStamp:HH:mm:ss.fff} {printerStatusUpdate.PrinterName} status updated: {printerStatusUpdate.PrinterStatus}");

                return;
            }

            parsed |= LaneStatusUpdateMessage.TryParse(line, LogTimeStamp, out var laneStatusUpdate);
            if (parsed)
            {
                _console.WriteLine($"Lanes updated: {string.Join(", ", laneStatusUpdate.LaneStatuses)}");
                return;
            }

            parsed |= ZonesFoundMessage.TryParse(line, LogTimeStamp, out var zonesFoundResult);
            if (parsed)
            {
                _console.WriteLine($"Zones found: {string.Join(", ", zonesFoundResult.ZoneIDs)}");
                return;
            }

            return; // TODO: throw exception for unknown message types
        }

        #endregion
    }
}