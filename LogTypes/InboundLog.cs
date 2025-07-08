using LogParser.Devices.ViewModel;
using LogParser.Messages;
using LogParser.Systems.Model;
using LogParser.Systems.ViewModel;
using System.Collections.ObjectModel;

namespace LogParser.LogTypes
{
    internal class InboundLog(InboundSystemViewModel viewModel, (Type MessageType, bool IsEnabled)[]? enabledMessages = null,  LogReaderConsole? console = null)
        : LogBase<InboundSystemModel, InboundSystemViewModel>(viewModel, enabledMessages, console)
    {
        #region Methods

        public override void ProcessLine(string line)
        {
            bool parsed = false;
            DateTimeOffset logTimeStamp = LogTimeStamp ?? DateTimeOffset.MinValue;

            if (EnabledMessages.TryGetValue(typeof(PrinterStatusUpdateMessage), out bool isEnabled) && isEnabled == true)
            {
                parsed |= PrinterStatusUpdateMessage.TryParse(line, logTimeStamp, out var printerStatusUpdate);
                if (parsed)
                {
                    // update state
                    ViewModel.Printers[printerStatusUpdate.ConveyorLineNumber].Status = printerStatusUpdate.PrinterStatus;
                    ViewModel.TimeStamp = printerStatusUpdate.EventTimeStamp;

                    // store history
                    _inboundHistory.Add(ViewModel.Model);

                    // update console
                    _console?.WriteLine($"{ViewModel.TimeStamp:HH:mm:ss.fff} {printerStatusUpdate.PrinterName} status updated: {printerStatusUpdate.PrinterStatus}");

                    return;
                }
            }

            if (EnabledMessages.TryGetValue(typeof(LaneStatusUpdateMessage), out isEnabled) && isEnabled == true)
            {
                parsed |= LaneStatusUpdateMessage.TryParse(line, logTimeStamp, out var laneStatusUpdate);
                if (parsed)
                {
                    // TODO implement state update
                    ViewModel.TimeStamp = laneStatusUpdate.TimeStamp;
                    _console?.WriteLine($"{ViewModel.TimeStamp:HH:mm:ss.fff} Lanes updated: {string.Join(", ", laneStatusUpdate.LaneStatuses)}");
                    return;
                }
            }


            if (EnabledMessages.TryGetValue(typeof(ZonesFoundMessage), out isEnabled) && isEnabled == true)
            {
                parsed |= ZonesFoundMessage.TryParse(line, logTimeStamp, out var zonesFoundResult);
                if (parsed)
                {
                    // TODO implement state update
                    ViewModel.TimeStamp = zonesFoundResult.TimeStamp;
                    _console?.WriteLine($"{ViewModel.TimeStamp:HH:mm:ss.fff} Zones found: {string.Join(", ", zonesFoundResult.ZoneIDs)}");
                    return;
                }
            }

            if (EnabledMessages.TryGetValue(typeof(ScanQueuedUpMessage), out isEnabled) && isEnabled == true)
            {
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
                    var scanner = scanQueuedUpResult.ScannerType;
                    var container = ViewModel.QueuedContainersLookup[scanner].LastOrDefault();
                    _console?.WriteLine($"{ViewModel.TimeStamp:HH:mm:ss.fff} Container queued up: {container?.LPN} ({container?.LotNumber}) at {scanner} Scanner");

                    return;
                }
            }

            return; // TODO: throw exception for unknown message types
        }

        #endregion
    }
}