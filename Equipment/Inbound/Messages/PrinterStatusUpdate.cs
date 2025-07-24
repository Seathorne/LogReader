using LogParser.Devices.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogParser.Equipment.Inbound.Messages;

internal partial record PrinterStatusUpdateMessage : InboundMessageBase<PrinterStatusUpdateMessage>, IParsable<PrinterStatusUpdateMessage>
{
    #region Constants

    public const string MessagePattern = $"{TimeStampPattern} {ThreadNumberPattern} {ThreadNullPattern} {MessageLevelPattern}  {EquipmentNumberPattern} - {EventTimeStampPattern} - {PrinterStatusPattern} {TagPattern}";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string PrinterStatusPattern = @"Update Printer Receiving Line (?<line>1|2) PandA (?<printer>\d) Enabled (?<status>True|False)";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string TagPattern = @"using Tag: (?<tag>SCANNER_(?<scanner>\d+)_PRINTER_W_STATUS\[(?<index>\d+)\])";

    #endregion

    #region Properties

    public static new MessageLevel MessageLevel => MessageLevel.Info;

    public TimeOnly EventTime { get; init; }

    public int PrinterID { get; init; }

    public bool IsEnabled { get; init; }

    public int LineID { get; init; }

    public int ScannerID { get; init; }

    public string TagName { get; init; }

    public int TagIndex { get; init; }

    public string PrinterName => $"PandA {PrinterID} (Receiving Line {LineID})";

    [GeneratedRegex(MessagePattern)]
    private static partial Regex MessageRegex { get; }

    #endregion

    #region Constructors

    public PrinterStatusUpdateMessage() : this(TimeOnly.MinValue, TimeOnly.MinValue, -1, -1, -1, false, -1, -1, "", -1)
    {
    }

    public PrinterStatusUpdateMessage(
        TimeOnly messageTime,
        TimeOnly eventTime,
        int threadID,
        int equipmentID,
        int printerID,
        bool isEnabled,
        int lineID,
        int scannerID,
        string tagName,
        int tagIndex)
    {
        MessageTime = messageTime;
        EventTime = eventTime;
        ThreadID = threadID;
        EquipmentID = equipmentID;
        PrinterID = printerID;
        IsEnabled = isEnabled;
        LineID = lineID;
        ScannerID = scannerID;
        TagName = tagName;
        TagIndex = tagIndex;
    }

    #endregion

    #region Methods

    public static PrinterStatusUpdateMessage Parse(string input)
        => Parse(input, CultureInfo.InvariantCulture);

    public static bool TryParse(string input, [MaybeNullWhen(false)] out PrinterStatusUpdateMessage result)
        => TryParse(input, CultureInfo.InvariantCulture, out result);

    public static PrinterStatusUpdateMessage Parse(string input, IFormatProvider? provider)
    {
        var match = MessageRegex.Match(input);
        var groups = match.Groups;

        if (!match.Success)
        {
            return new PrinterStatusUpdateMessage();
        }
        else
        {
            return new PrinterStatusUpdateMessage(
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

                printerID: int.Parse(groups["printer"].Value),
                isEnabled: bool.Parse(groups["status"].Value),

                lineID: int.Parse(groups["line"].Value),
                scannerID: int.Parse(groups["scanner"].Value),
                tagName: groups["tag"].Value,
                tagIndex: int.Parse(groups["index"].Value)
            );
        }
    }

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out PrinterStatusUpdateMessage result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var match = MessageRegex.Match(input);
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

        if (!int.TryParse(groups["printer"].Value, out int printerID))
            return false;

        if (!bool.TryParse(groups["status"].Value, out bool isEnabled))
            return false;

        if (!int.TryParse(groups["line"].Value, out int lineID))
            return false;

        if (!int.TryParse(groups["scanner"].Value, out int scannerID))
            return false;

        if (!int.TryParse(groups["index"].Value, out int tagIndex))
            return false;

        result = new PrinterStatusUpdateMessage(
            messageTime: new TimeOnly(hour, minute, second, millisecond),
            eventTime: new TimeOnly(eventHour, eventMinute, eventSecond, eventMillisecond),
            threadID: threadID,
            equipmentID: equipmentID,
            printerID: printerID,
            isEnabled: isEnabled,
            lineID: lineID,
            scannerID: scannerID,
            tagName: groups["tag"].Value,
            tagIndex: tagIndex
        );

        return true;
    }

    #endregion
}
