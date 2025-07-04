using LogParser.Devices.Enum;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LogParser.Messages
{
    internal partial record LaneStatusUpdateMessage(DateTimeOffset TimeStamp,
        int ThreadNumber, MessageLevel MessageLevel, int EquipmentNumber, ImmutableArray<(int LaneNumber, LaneStatus LaneStatus)> LaneStatuses, int ConveyorLineNumber) : MessageBase
    {
        #region Private Constants

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string DisabledLanesPattern = @"(?<status>Disabled|Partially Full|Full) Lanes \[(?<lanes>(?:(?<lane>\d+),?)*)\]";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string ScannerPattern = @"at Scanner \[Line (?<line>\d+) Receiving Scanner\]";

        #endregion

        #region Public Constants

        public const string MessagePattern = $"{TimeStampPattern} {ThreadNumberPattern} {ThreadNullPattern} {MessageLevelPattern} {EquipmentNumberPattern} - {DisabledLanesPattern} {ScannerPattern}";

        #endregion

        #region Public Properties

        [GeneratedRegex(MessagePattern)]
        private static partial Regex MessageRegex { get; }

        public string ScannerName => $"Line {ConveyorLineNumber} Receiving Scanner";

        #endregion

        #region Public Static Properties

        public static LaneStatusUpdateMessage DefaultRecord => new LaneStatusUpdateMessage(DateTimeOffset.MinValue, -1, MessageLevel.None, -1, [], -1);

        #endregion

        #region Public Static Methods

        public static bool TryParse(string message, DateTime logTimeStamp, TimeZoneInfo timeZone, out LaneStatusUpdateMessage result)
        {
            var match = MessageRegex.Match(message);

            if (!match.Success)
            {
                result = DefaultRecord;
                return false;
            }
            else
            {
                var utcOffset = timeZone.GetUtcOffset(logTimeStamp);
                var messageType = Enum.Parse<LaneStatus>(ToPascalCase(match.Groups["status"].Value));
                var messageLevel = Enum.Parse<MessageLevel>(ToPascalCase(match.Groups["level"].Value));

                result = new LaneStatusUpdateMessage
                    (
                        TimeStamp: new DateTimeOffset
                            (
                                year: logTimeStamp.Year,
                                month: logTimeStamp.Month,
                                day: logTimeStamp.Day,
                                hour: int.Parse(match.Groups["hour"].Value),
                                minute: int.Parse(match.Groups["minute"].Value),
                                second: int.Parse(match.Groups["second"].Value),
                                millisecond: int.Parse(match.Groups["millisecond"].Value),
                                offset: utcOffset
                            ),

                        ThreadNumber: int.Parse(match.Groups["thread"].Value),
                        MessageLevel: messageLevel,

                        EquipmentNumber: int.Parse(match.Groups["equipment"].Value),
                        LaneStatuses: getLaneStatuses(messageType),
                        ConveyorLineNumber: int.Parse(match.Groups["line"].Value)
                    );

                return true;
            }

            ImmutableArray<(int LaneNumber, LaneStatus LaneStatus)> getLaneStatuses(LaneStatus messageType)
            {
                var laneCaptures = match.Groups["lane"].Captures;
                int laneCount = laneCaptures.Count >= 1 ? int.Parse(laneCaptures.Last().Value) : 0;
                var laneStatuses = new (int LaneNumber, LaneStatus LaneStatus)[laneCount];

                if (laneCount > 0)
                {
                    for (int i = 0; i < laneCaptures.Count; i++)
                    {
                        int laneNumber = int.Parse(laneCaptures[i].Value);
                        int laneIndex = laneNumber - 1;
                        laneStatuses[laneIndex].LaneNumber = laneNumber;
                        laneStatuses[laneIndex].LaneStatus = messageType;
                    }
                }

                return ImmutableArray.Create(laneStatuses);
            }
        }

        #endregion
    }
}