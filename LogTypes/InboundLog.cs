using LogParser.Devices.ViewModel;
using LogParser.Messages;
using LogParser.Systems.Model;
using LogParser.Systems.ViewModel;
using System.Collections.ObjectModel;

namespace LogParser.LogTypes
{
    internal class InboundLog(InboundSystemViewModel viewModel, LogReaderConsole? console = null)
        : LogBase<InboundSystemModel, InboundSystemViewModel>(viewModel, console)
    {
        #region Methods

        public override void ProcessLine(string line)
        {
            bool parsed = false;
            DateTimeOffset logTimeStamp = LogTimeStamp ?? DateTimeOffset.MinValue;

            parsed |= PrinterStatusUpdateMessage.TryParse(line, logTimeStamp, out var printerStatusUpdate);
            if (parsed)
            {
                int index = printerStatusUpdate.ConveyorLineNumber - 1;

                // update state
                ViewModel.Printers[index].Status = printerStatusUpdate.PrinterStatus;
                ViewModel.TimeStamp = printerStatusUpdate.EventTimeStamp;

                // store history
                _inboundHistory.Add(ViewModel.Model);

                // update console
                _console?.WriteLine($"{ViewModel.TimeStamp:HH:mm:ss.fff} {printerStatusUpdate.PrinterName} status updated: {printerStatusUpdate.PrinterStatus}");

                return;
            }

            parsed |= LaneStatusUpdateMessage.TryParse(line, logTimeStamp, out var laneStatusUpdate);
            if (parsed)
            {
                // TODO implement state update
                ViewModel.TimeStamp = laneStatusUpdate.TimeStamp;
                _console?.WriteLine($"{ViewModel.TimeStamp:HH:mm:ss.fff} Lanes updated: {string.Join(", ", laneStatusUpdate.LaneStatuses)}");
                return;
            }

            parsed |= ZonesFoundMessage.TryParse(line, logTimeStamp, out var zonesFoundResult);
            if (parsed)
            {
                // TODO implement state update
                ViewModel.TimeStamp = zonesFoundResult.TimeStamp;
                _console?.WriteLine($"{ViewModel.TimeStamp:HH:mm:ss.fff} Zones found: {string.Join(", ", zonesFoundResult.ZoneIDs)}");
                return;
            }

            parsed |= ScanQueuedUpMessage.TryParse(line, logTimeStamp, out var scanQueuedUpResult);
            if (parsed)
            {
                string[] barcodes = [.. scanQueuedUpResult.Barcodes];

                // update state
                if (ViewModel.QueuedUpContainers.TryGetValue(scanQueuedUpResult.ScannerType, out ObservableCollection<ContainerViewModel>? queuedContainers))
                    queuedContainers.Add(new ContainerViewModel(barcodes));
                ViewModel.TimeStamp = scanQueuedUpResult.EventTimeStamp;

                // store history
                _inboundHistory.Add(ViewModel.Model);

                // update console
                foreach (var scanner in ViewModel.QueuedContainersLookup)
                {
                    foreach (var container in scanner)
                    {
                        _console?.WriteLine($"{ViewModel.TimeStamp:HH:mm:ss.fff} Container queued up: {container?.LPN} ({container?.LotNumber}) at {scanner.Key} Scanner");
                    }
                }

                return;
            }

            return; // TODO: throw exception for unknown message types
        }

        #endregion
    }
}