using LogParser.Devices.Enums;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogParser.Equipment.Inbound.Messages;

internal partial record ContainerScannedMessage : InboundMessageBase<ContainerScannedMessage>, IParsable<ContainerScannedMessage>
{
    #region Constants

    public const string MessagePattern = $"{TimeStampPattern} {ThreadNumberPattern} {ThreadNullPattern} {MessageLevelPattern}  {EquipmentNumberPattern} - {EventTimeStampPattern} - {BarcodePattern} {ScannerPattern}";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string BarcodePattern = @"SCAN (?<barcodes>(?:(?<barcode>\w*\-?\d+),?\s?)*)";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string ScannerPattern = @"Queued UP For Scanner Line (?<line>1|2) (?<scanner>Receiving|Verification) Scanner TIME START";

    #endregion

    #region Properties

    public TimeOnly EventTime { get; init; }

    public int LineID { get; init; }

    public ScannerName ScannerName { get; init; }

    public ImmutableArray<string> Barcodes { get; init; }

    public string ScannerFullName => $"Line {LineID} {ScannerName} Scanner";

    [GeneratedRegex(MessagePattern)]
    private static partial Regex ScanQueuedUpRegex { get; }

    #endregion

    #region Constructors

    public ContainerScannedMessage() : this(TimeOnly.MinValue, TimeOnly.MinValue, -1, -1, -1, ScannerName.None, [])
    {
    }

    public ContainerScannedMessage(
        TimeOnly messageTime,
        TimeOnly eventTime,
        int threadID,
        int equipmentID,
        int lineID,
        ScannerName scannerName,
        ImmutableArray<string> barcodes)
    {
        MessageTime = messageTime;
        EventTime = eventTime;
        ThreadID = threadID;
        EquipmentID = equipmentID;
        LineID = lineID;
        ScannerName = scannerName;
        Barcodes = barcodes;
    }

    #endregion

    #region Methods

    public static ContainerScannedMessage Parse(string input)
        => Parse(input, CultureInfo.InvariantCulture);

    public static bool TryParse(string input, [MaybeNullWhen(false)] out ContainerScannedMessage result)
        => TryParse(input, CultureInfo.InvariantCulture, out result);

    public static ContainerScannedMessage Parse(string input, IFormatProvider? provider)
    {
        var match = ScanQueuedUpRegex.Match(input);
        var groups = match.Groups;

        if (!match.Success)
        {
            return new ContainerScannedMessage();
        }
        else
        {
            var barcodes = groups["barcode"].Captures.Select(capture => capture.Value);

            return new ContainerScannedMessage (
                messageTime: new TimeOnly(
                    hour: int.Parse(groups["hour"].Value),
                    minute: int.Parse(groups["minute"].Value),
                    second: int.Parse(groups["second"].Value),
                    millisecond: int.Parse(groups["millisecond"].Value)
                ),

                eventTime: new TimeOnly(
                    hour: int.Parse(groups["_hour"].Value),
                    minute: int.Parse(groups["_minute"].Value),
                    second: int.Parse(groups["_second"].Value),
                    millisecond: int.Parse(groups["_millisecond"].Value)
                ),

                threadID: int.Parse(groups["thread"].Value),
                equipmentID: int.Parse(groups["equipment"].Value),
                lineID: int.Parse(groups["line"].Value),

                scannerName: Enum.Parse<ScannerName>(groups["scanner"].Value),
                barcodes: [.. barcodes]
            );
        }
    }

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out ContainerScannedMessage result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var match = ScanQueuedUpRegex.Match(input);
        var groups = match.Groups;

        if (!match.Success)
            return false;

        if (!int.TryParse(groups["hour"].Value, out int hour))
            return false;

        if (!int.TryParse(groups["minute"].Value, out int minute))
            return false;

        if (!int.TryParse(groups["second"].Value, out int second))
            return false;

        if (!int.TryParse(groups["millisecond"].Value, out int millisecond))
            return false;

        if (!int.TryParse(groups["_hour"].Value, out int eventHour))
            return false;

        if (!int.TryParse(groups["_minute"].Value, out int eventMinute))
            return false;

        if (!int.TryParse(groups["_second"].Value, out int eventSecond))
            return false;

        if (!int.TryParse(groups["_millisecond"].Value, out int eventMillisecond))
            return false;

        if (!int.TryParse(groups["thread"].Value, out int threadID))
            return false;

        if (!int.TryParse(groups["equipment"].Value, out int equipmentID))
            return false;

        if (!int.TryParse(groups["line"].Value, out int lineID))
            return false;

        if (!Enum.TryParse<ScannerName>(groups["scanner"].Value, out var scannerName))
            return false;

        var barcodes = groups["barcode"].Captures.Select(capture => capture.Value);

        result = new ContainerScannedMessage(
            messageTime: new TimeOnly(hour, minute, second, millisecond),
            eventTime: new TimeOnly(eventHour, eventMinute, eventSecond, eventMillisecond),
            threadID,
            equipmentID,
            lineID,
            scannerName,
            barcodes: [.. barcodes]
        );

        return true;
    }

    #endregion
}