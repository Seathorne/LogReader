using LogParser.Common.Records;
using LogParser.Devices.Enums;
using LogParser.Devices.Tracking;
using LogParser.Equipment.Common.Enums;
using LogParser.Equipment.Common.Exceptions;
using LogParser.Equipment.Common.Messages;
using LogParser.Equipment.Common.Parsers;
using LogParser.Equipment.Inbound.Messages;
using LogParser.Subsystems.Models;
using LogParser.Subsystems.Tracking;
using LogParser.System;
using System.Collections.Immutable;

namespace LogParser.Equipment.Inbound.Logs;

internal class InboundLog : LogBase<InboundSubsystemModel, TrackedInboundSubsystem>
{
    #region Constructors

    public InboundLog(TrackedInboundSubsystem inboundSubsystem, Dictionary<MessageClass, bool> enabledMessages, LogReaderConsole? console = null)
        : base(inboundSubsystem, enabledMessages, console)
    {
        MessageProcessors.Enqueue(ProcessPrinterStatusUpdateMessage);
        MessageProcessors.Enqueue(ProcessLaneStatusUpdateMessage);
        MessageProcessors.Enqueue(ProcessZonesFoundMessage);
    }

    #endregion

    #region Methods

    private IParseableMessage? ProcessPrinterStatusUpdateMessage(string input, int lineNumber, ref bool parsed)
    {
        if (!_enabledMessages.TryGetValue(MessageClass.PrinterStatusUpdateMessage, out bool isEnabled) || isEnabled != true)
            return null;

        parsed |= PrinterStatusUpdateMessage.TryParse(input, out var printerStatusUpdate);
        if (parsed && printerStatusUpdate != null)
        {
            int index = printerStatusUpdate.LineID;
            var status = printerStatusUpdate.IsEnabled ? PrinterStatus.On : PrinterStatus.Off;

            // update state
            var printer = TrackedSubsystem.Printers[index];
            using (printer.WithTimestamp(GetTimeStamp(printerStatusUpdate.EventTime, lineNumber)))
            {
                printer.Status = status;
            }

            // store history
            _inboundHistory.Add(TrackedSubsystem.Model);

            // update console
            _console?.WriteLine($"{printerStatusUpdate.EventTime:HH:mm:ss.fff} {printerStatusUpdate.PrinterName} status updated: {status}");

            return printerStatusUpdate;
        }
        else return null;
    }

    private IParseableMessage? ProcessLaneStatusUpdateMessage(string input, int lineNumber, ref bool parsed)
    {
        if (!_enabledMessages.TryGetValue(MessageClass.LaneStatusUpdateMessage, out bool isEnabled) || isEnabled != true)
            return null;

        parsed |= LaneStatusUpdateMessage.TryParse(input, out var laneStatusUpdate);
        if (parsed && laneStatusUpdate != null)
        {
            // TODO: update state

            // update console
            _console?.WriteLine($"{laneStatusUpdate.MessageTime:HH:mm:ss.fff} Lanes updated: {string.Join(", ", laneStatusUpdate.Lanes)}");
            return laneStatusUpdate;
        }
        else return null;
    }

    private IParseableMessage? ProcessZonesFoundMessage(string input, int lineNumber, ref bool parsed)
    {
        if (!_enabledMessages.TryGetValue(MessageClass.ZonesFoundMessage, out bool isEnabled) || isEnabled != true)
            return null;

        parsed |= ZonesFoundMessage.TryParse(input, out var zonesFoundResult);
        if (parsed && zonesFoundResult != null)
        {
            // TODO: update state

            _console?.WriteLine($"{zonesFoundResult.MessageTime:HH:mm:ss.fff} Zones found: {string.Join(", ", zonesFoundResult.Zones)}");
            return zonesFoundResult;
        }
        else return null;
    }

    private IParseableMessage? ProcessContainerScannedMessage(string input, int lineNumber, ref bool parsed)
    {
        if (!_enabledMessages.TryGetValue(MessageClass.ContainerScannedMessage, out bool isEnabled) || isEnabled != true)
            return null;

        parsed |= ContainerScannedMessage.TryParse(input, out var containerScannedResult);
        if (parsed && containerScannedResult != null)
        {
            string[] barcodes = [.. containerScannedResult.Barcodes];
            var scanner = containerScannedResult.ScannerName;
            Container container;

            // update state
            if (TrackedSubsystem.Scanners.TryGetValue(scanner, out TrackedScanner? trackedScanner) && trackedScanner is not null)
            {
                container = Container.FromBarcodes(barcodes);
                using (TrackedSubsystem.WithTimestamp(GetTimeStamp(containerScannedResult.EventTime, lineNumber)))
                {
                    // TODO: fix TrackedScanner to store and notify single-container delta not entire property
                    trackedScanner.ScannedContainers = ImmutableQueue.Create([.. trackedScanner.ScannedContainers, container]);
                }
            }
            else throw new InvalidOperationException($"Scanner {scanner} does not exist!");
            // TODO: verify container == TrackedSubsystem.Scanners[scanner].ScannedContainers.LastOrDefault();

            // store history
            _inboundHistory.Add(TrackedSubsystem.Model);

            // update console
            _console?.WriteLine($"{containerScannedResult.EventTime:HH:mm:ss.fff} Container queued up: {container.Barcode} ({container.LotBarcode}) at {scanner} Scanner");

            return containerScannedResult;
        }
        else return null;
    }

    public override void ProcessLine(string input, int lineNumber)
    {
        bool parsed = false;
        foreach (var messageProcessor in MessageProcessors)
        {
            _ = messageProcessor.Invoke(input, lineNumber, ref parsed);
            if (parsed)
            {
                break;
            }
        }

        //if (parsed == false)
            //throw new UnknownMessageException(input);
        //else return;
    }

    #endregion
}