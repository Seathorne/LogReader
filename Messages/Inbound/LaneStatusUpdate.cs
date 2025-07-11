using LogParser.Devices.Enum;
using LogParser.Devices.Model;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogParser.Messages
{
    internal partial record LaneStatusUpdate : MessageBase<LaneStatusUpdate>, IParsable<LaneStatusUpdate>
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

        public ImmutableArray<LaneModel> Lanes { get; init; }

        public int LineID { get; init; }

        public string ScannerName => $"Line {LineID} Receiving Scanner";

        [GeneratedRegex(MessagePattern)]
        private static partial Regex MessageRegex { get; }

        #endregion

        #region Constructors

        public LaneStatusUpdate() : this(TimeOnly.MinValue, -1, -1, -1, [])
        {
        }

        public LaneStatusUpdate(
            TimeOnly messageTime,
            int threadID,
            int equipmentID,
            int lineID,
            ISet<LaneModel> laneModels) : this(messageTime, threadID, equipmentID, lineID, [.. laneModels])
        {
        }

        public LaneStatusUpdate(
            TimeOnly messageTime,
            int threadID,
            int equipmentID,
            int lineID,
            ImmutableArray<LaneModel> lanes)
        {
            MessageTime = messageTime;
            ThreadID = threadID;
            EquipmentID = equipmentID;
            Lanes = lanes;
            LineID = lineID;
        }

        #endregion

        #region Methods

        public static LaneStatusUpdate Parse(string message)
            => Parse(message, CultureInfo.InvariantCulture);

        public static bool TryParse(string message, [MaybeNullWhen(false)] out LaneStatusUpdate result)
            => TryParse(message, CultureInfo.InvariantCulture, out result);

        public static LaneStatusUpdate Parse(string input, IFormatProvider? provider)
        {
            var match = MessageRegex.Match(input);
            var groups = match.Groups;

            if (!match.Success)
            {
                return new LaneStatusUpdate();
            }
            else
            {
                var laneStatus = Enum.Parse<LaneStatus>(ToPascalCase(groups["status"].Value));
                var laneModels = groups["lane"].Captures.Select(laneID => new LaneModel(int.Parse(laneID.Value), laneStatus));

                return new LaneStatusUpdate(
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

        public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out LaneStatusUpdate result)
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

            IList<LaneModel> laneModels = [];
            foreach (Capture capture in groups["lane"].Captures)
            {
                if (!int.TryParse(capture.Value, out int laneID))
                    return false;

                laneModels.Add(new LaneModel(laneID, laneStatus));
            }

            result = new LaneStatusUpdate(
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
}