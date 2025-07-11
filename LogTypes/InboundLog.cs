using LogParser.Devices.ViewModel;
using LogParser.Messages;
using LogParser.Messages.Inbound;
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

            if (EnabledMessages.TryGetValue(typeof(PrinterStatusUpdate), out bool isEnabled) && isEnabled == true)
            {
                parsed |= PrinterStatusUpdate.TryParse(line, out var printerStatusUpdate);
                if (parsed && printerStatusUpdate != null)
                {
                    int index = printerStatusUpdate.LineID;

                    // update state
                    ViewModel.Printers[index].Status = printerStatusUpdate.Printer.Status;

                    // store history
                    _inboundHistory.Add(ViewModel.Model);

                    // update console
                    _console?.WriteLine($"{printerStatusUpdate.EventTime:HH:mm:ss.fff} {printerStatusUpdate.PrinterName} status updated: {printerStatusUpdate.Printer.Status}");

                    return;
                }
            }

            if (EnabledMessages.TryGetValue(typeof(LaneStatusUpdate), out isEnabled) && isEnabled == true)
            {
                parsed |= LaneStatusUpdate.TryParse(line, out var laneStatusUpdate);
                if (parsed && laneStatusUpdate != null)
                {
                    // foreach (LaneModel lane in laneStatusUpdate.Lanes)
                    // {
                        // ViewModel.Lanes[lane.LaneID].Status = lane.Status;
                    // }
                    // _inboundHistory.Add(ViewModel.Model);

                    _console?.WriteLine($"{laneStatusUpdate.MessageTime:HH:mm:ss.fff} Lanes updated: {string.Join(", ", laneStatusUpdate.Lanes)}");
                    return;
                }
            }


            if (EnabledMessages.TryGetValue(typeof(ZonesFoundMessage), out isEnabled) && isEnabled == true)
            {
                parsed |= ZonesFoundMessage.TryParse(line, out var zonesFoundResult);
                if (parsed && zonesFoundResult != null)
                {
                    // foreach (ZoneModel zone in zonesFoundResult.Zones)
                    // {
                        // ViewModel.Zones[zone.ZoneID].Status = zone.Status;
                    // }
                    // _inboundHistory.Add(ViewModel.Model);

                    _console?.WriteLine($"{zonesFoundResult.MessageTime:HH:mm:ss.fff} Zones found: {string.Join(", ", zonesFoundResult.Zones)}");
                    return;
                }
            }

            if (EnabledMessages.TryGetValue(typeof(ScanQueuedUpMessage), out isEnabled) && isEnabled == true)
            {
                parsed |= ScanQueuedUpMessage.TryParse(line, out var scanQueuedUpResult);
                if (parsed && scanQueuedUpResult != null)
                {
                    string[] barcodes = [.. scanQueuedUpResult.Barcodes];
                    var scanner = scanQueuedUpResult.ScannerName;

                    // update state
                    if (ViewModel.QueuedContainers.TryGetValue(scanner, out ObservableCollection<ContainerViewModel>? queuedContainers))
                        queuedContainers.Add(new ContainerViewModel(barcodes));

                    // store history
                    _inboundHistory.Add(ViewModel.Model);

                    // update console
                    var container = ViewModel.QueuedContainers[scanner].LastOrDefault();
                    _console?.WriteLine($"{scanQueuedUpResult.EventTime:HH:mm:ss.fff} Container queued up: {container?.LPN} ({container?.LotNumber}) at {scanner} Scanner");

                    return;
                }
            }

            return; // TODO: throw exception for unknown message types
        }

        #endregion
    }
}