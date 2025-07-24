using LogParser.Devices.Enums;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogParser.Equipment.Inbound.Messages;

internal partial record LaneStatusUpdateMessage : InboundMessageBase<LaneStatusUpdateMessage>, IParsable<LaneStatusUpdateMessage>
{
    #region Constants

    public const string MessagePattern = $"{TimeStampPattern} {ThreadNumberPattern} {ThreadNullPattern} {MessageLevelPattern} {EquipmentNumberPattern} - {DisabledLanesPattern} {ScannerPattern}";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string DisabledLanesPattern = @"(?<status>Disabled|Partially Full|Full) Lanes \[(?<lanes>(?:(?<lane>\d+),?\s?)*)\]";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string ScannerPattern = @"at Scanner \[Line (?<line>\d+) Receiving Scanner\]";

    #endregion

    #region Properties

    public static new MessageLevel MessageLevel => MessageLevel.Debug;

    public ImmutableArray<int> Lanes { get; init; }

    public int LineID { get; init; }

    public string ScannerName => $"Line {LineID} Receiving Scanner";

    [GeneratedRegex(MessagePattern)]
    private static partial Regex MessageRegex { get; }

    #endregion

    #region Constructors

    public LaneStatusUpdateMessage() : this(TimeOnly.MinValue, -1, -1, -1, [])
    {
    }

    public LaneStatusUpdateMessage(
        TimeOnly messageTime,
        int threadID,
        int equipmentID,
        int lineID,
        ImmutableArray<int> lanes)
    {
        MessageTime = messageTime;
        ThreadID = threadID;
        EquipmentID = equipmentID;
        LineID = lineID;
        Lanes = lanes;
    }

    #endregion

    #region Methods

    public static LaneStatusUpdateMessage Parse(string message)
        => Parse(message, CultureInfo.InvariantCulture);

    public static bool TryParse(string message, [MaybeNullWhen(false)] out LaneStatusUpdateMessage result)
        => TryParse(message, CultureInfo.InvariantCulture, out result);

    public static LaneStatusUpdateMessage Parse(string input, IFormatProvider? provider)
    {
        var match = MessageRegex.Match(input);
        var groups = match.Groups;

        if (!match.Success)
        {
            return new LaneStatusUpdateMessage();
        }
        else
        {
            var laneStatus = Enum.Parse<LaneStatus>(ToPascalCase(groups["status"].Value));
            var laneModels = groups["lane"].Captures.Select(capture => int.Parse(capture.Value));

            return new LaneStatusUpdateMessage(
                messageTime: new TimeOnly(
                    hour: int.Parse(groups["hour"].Value),
                    minute: int.Parse(groups["minute"].Value),
                    second: int.Parse(groups["second"].Value),
                    millisecond: int.Parse(groups["millisecond"].Value)
                ),

                threadID: int.Parse(groups["thread"].Value),
                equipmentID: int.Parse(groups["equipment"].Value),
                lineID: int.Parse(groups["line"].Value),

                lanes: [.. laneModels]
            );
        }
    }

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out LaneStatusUpdateMessage result)
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

        if (!Enum.TryParse<LaneStatus>(ToPascalCase(groups["status"].Value), out var laneStatus))
            return false;

        IList<int> laneModels = [];
        foreach (Capture capture in groups["lane"].Captures)
        {
            if (!int.TryParse(capture.Value, out int laneID))
                return false;

            laneModels.Add(laneID);
        }

        result = new LaneStatusUpdateMessage(
            messageTime: new TimeOnly(hour, minute, second, millisecond),
            threadID,
            equipmentID,
            lineID,
            lanes: [.. laneModels]
        );

        return true;
    }

    #endregion
}