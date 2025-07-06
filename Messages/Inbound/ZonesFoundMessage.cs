using LogParser.Devices.Enum;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LogParser.Messages
{
    internal partial record ZonesFoundMessage(DateTimeOffset TimeStamp,
        int ThreadNumber, MessageLevel MessageLevel, int EquipmentNumber, ImmutableArray<string> ZoneIDs, int ConveyorLineNumber) : MessageBase
    {
        #region Constants

        public const string MessagePattern = $"{TimeStampPattern} {ThreadNumberPattern} {ThreadNullPattern} {MessageLevelPattern} {EquipmentNumberPattern} - {ZonesFoundPattern} {ScannerPattern}";
        
        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string ZonesFoundPattern = @"Zones Found \[(?<zones>(?:(?<zone>\d+),?\s?)*)\]";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string ScannerPattern = @"at Scanner \[Line (?<line>\d+) Receiving Scanner\]";

        #endregion

        #region Public Properties

        [GeneratedRegex(MessagePattern)]
        private static partial Regex MessageRegex { get; }

        public string ScannerName => $"Line {ConveyorLineNumber} Receiving Scanner";

        #endregion

        #region Properties

        public static ZonesFoundMessage DefaultRecord => new ZonesFoundMessage(DateTimeOffset.MinValue, -1, MessageLevel.None, -1, [], -1);

        #endregion

        #region Methods

        public static bool TryParse(string message, DateTimeOffset logTimeStamp, out ZonesFoundMessage result)
        {
            var match = MessageRegex.Match(message);

            if (!match.Success)
            {
                result = DefaultRecord;
                return false;
            }
            else
            {
                var messageLevel = Enum.Parse<MessageLevel>(ToPascalCase(match.Groups["level"].Value));
                var zoneNumbers = ImmutableArray.Create(match.Groups["zone"].Captures.Select(capture => capture.Value).ToArray());

                result = new ZonesFoundMessage
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
                                offset: logTimeStamp.Offset
                            ),

                        ThreadNumber: int.Parse(match.Groups["thread"].Value),
                        MessageLevel: messageLevel,

                        EquipmentNumber: int.Parse(match.Groups["equipment"].Value),
                        ZoneIDs: zoneNumbers,
                        ConveyorLineNumber: int.Parse(match.Groups["line"].Value)
                    );

                return true;
            }
        }

        #endregion
    }
}