using LogParser.Devices.Enums;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogParser.Equipment.Inbound.Messages;

internal partial record ZonesFoundMessage : InboundMessageBase<ZonesFoundMessage>, IParsable<ZonesFoundMessage>
{
    #region Constants

    public const string MessagePattern = $"{TimeStampPattern} {ThreadNumberPattern} {ThreadNullPattern} {MessageLevelPattern} {EquipmentNumberPattern} - {ZonesFoundPattern} {ScannerPattern}";
    
    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string ZonesFoundPattern = @"Zones Found \[(?<zones>(?:(?<zone>\d+),?\s?)*)\]";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string ScannerPattern = @"at Scanner \[Line (?<line>\d+) Receiving Scanner\]";

    #endregion

    #region Properties

    public static new MessageLevel MessageLevel = MessageLevel.Debug;

    public ImmutableArray<string> Zones { get; }

    public int LineID { get; init; }
    
    public string ScannerName => $"Line {LineID} Receiving Scanner";

    [GeneratedRegex(MessagePattern)]
    private static partial Regex MessageRegex { get; }

    #endregion

    #region Constructors

    public ZonesFoundMessage() : this(TimeOnly.MinValue, -1, -1, -1, [])
    {
    }

    public ZonesFoundMessage(
        TimeOnly messageTime,
        int threadID,
        int equipmentID,
        int lineID,
        ImmutableArray<string> zoneModels)
    {
        MessageTime = messageTime;
        ThreadID = threadID;
        EquipmentID = equipmentID;
        LineID = lineID;
        Zones = zoneModels;
    }

    #endregion

    #region Methods

    public static ZonesFoundMessage Parse(string input)
        => Parse(input, CultureInfo.InvariantCulture);

    public static bool TryParse(string input, [MaybeNullWhen(false)] out ZonesFoundMessage result)
        => TryParse(input, CultureInfo.InvariantCulture, out result);

    public static ZonesFoundMessage Parse(string input, IFormatProvider? provider)
    {
        var match = MessageRegex.Match(input);
        var groups = match.Groups;

        if (!match.Success)
        {
            return new ZonesFoundMessage();
        }
        else
        {
            var messageLevel = Enum.Parse<MessageLevel>(ToPascalCase(groups["level"].Value));
            var zoneModels = groups["zone"].Captures.Select(capture => capture.Value);

            return new ZonesFoundMessage(
                messageTime: new TimeOnly(
                    hour: int.Parse(groups["hour"].Value),
                    minute: int.Parse(groups["minute"].Value),
                    second: int.Parse(groups["second"].Value),
                    millisecond: int.Parse(groups["millisecond"].Value)
                ),

                threadID: int.Parse(groups["thread"].Value),
                equipmentID: int.Parse(groups["equipment"].Value),
                lineID: int.Parse(groups["line"].Value),

                zoneModels: [.. zoneModels]
            );
        }
    }

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out ZonesFoundMessage result)
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

        if (!int.TryParse(groups["thread"].Value, out int threadID))
            return false;

        if (!int.TryParse(groups["equipment"].Value, out int equipmentID))
            return false;

        if (!int.TryParse(groups["line"].Value, out int lineID))
            return false;

        var zoneModels = groups["zone"].Captures.Select(capture => capture.Value);

        result = new ZonesFoundMessage(
            messageTime: new TimeOnly(hour, minute, second, millisecond),
            threadID,
            equipmentID,
            lineID,
            zoneModels: [.. zoneModels]
        );

        return true;
    }

    #endregion
}