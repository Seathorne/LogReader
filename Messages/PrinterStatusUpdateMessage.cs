using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LogParser.Messages
{
    internal record PrinterStatusUpdateMessage(DateTime LogTimeStamp, TimeSpan UtcOffset, int Hour, int Minutes, int Seconds, int Milliseconds, int EventHour, int EventMinutes, int EventSeconds,
        int EventMilliseconds, int ThreadNumber, MessageLevel MessageLevel, int EquipmentNumber, int ConveyorLineNumber, int PrinterNumber, bool PrinterStatus, string TagName, int ScannerNumber, int TagIndex) : BaseMessage
    {
        #region Private Constants

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string TimeStampPattern = @"(?<time>(?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})\.(?<millisecond>\d{3}))";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string EventTimeStampPattern = @"(?<_time>(?<_hour>\d{2}):(?<_minute>\d{2}):(?<_second>\d{2}):(?<_millisecond>\d{3}))";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string ThreadNumberPattern = @"\[(?<thread>\d+)\]";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string ThreadNullPattern = @"\(null\)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string MessageLevelPattern = @"(?<level>INFO|DEBUG|WARNING|ERROR)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string EquipmentNumberPattern = @"Equipment (?<equipment>\d+)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string PrinterStatusPattern = @"Update Printer Receiving Line (?<line>\d) PandA (?<printer>\d) Enabled (?<status>True|False)";
        
        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string TagPattern = @"using Tag: (?<tag>SCANNER_(?<scanner>\d+)_PRINTER_W_STATUS\[(?<index>\d+)\])";

        #endregion

        #region Public Properties

        public DateTimeOffset TimeStamp { get; } = new DateTimeOffset
            (
                year: LogTimeStamp.Year,
                month: LogTimeStamp.Month,
                day: LogTimeStamp.Day,
                hour: Hour,
                minute: Minutes,
                second: Seconds,
                millisecond: Milliseconds,
                offset: UtcOffset
            );

        public DateTimeOffset EventTimeStamp { get; } = new DateTimeOffset
            (
                year: LogTimeStamp.Year,
                month: LogTimeStamp.Month,
                day: LogTimeStamp.Day,
                hour: EventHour,
                minute: EventMinutes,
                second: EventSeconds,
                millisecond: EventMilliseconds,
                offset: UtcOffset
            );

        public string PrinterName => $"PandA {PrinterNumber} (Receiving Line {ConveyorLineNumber})";

        #endregion

        #region Public Static Methods

        public static bool TryParse(string message, DateTime logTimeStamp, TimeZoneInfo timeZone, out PrinterStatusUpdateMessage? result)
        {
            var pattern = string.Format(@"{0} {1} {2} {3}  {4} - {5} - {6} {7}",
                TimeStampPattern,
                ThreadNumberPattern,
                ThreadNullPattern,
                MessageLevelPattern,
                EquipmentNumberPattern,
                EventTimeStampPattern,
                PrinterStatusPattern,
                TagPattern);
            var match = Regex.Match(message, pattern);

            if (!match.Success)
            {
                result = default;
                return false;
            }
            else
            {
                result = new PrinterStatusUpdateMessage
                    (
                        LogTimeStamp: logTimeStamp,
                        UtcOffset: timeZone.GetUtcOffset(logTimeStamp),

                        Hour: int.Parse(match.Groups["hour"].Value),
                        Minutes: int.Parse(match.Groups["minute"].Value),
                        Seconds: int.Parse(match.Groups["second"].Value),
                        Milliseconds: int.Parse(match.Groups["millisecond"].Value),

                        EventHour: int.Parse(match.Groups["_hour"].Value),
                        EventMinutes: int.Parse(match.Groups["_minute"].Value),
                        EventSeconds: int.Parse(match.Groups["_second"].Value),
                        EventMilliseconds: int.Parse(match.Groups["_millisecond"].Value),

                        ThreadNumber: int.Parse(match.Groups["thread"].Value),
                        MessageLevel: Enum.Parse<MessageLevel>(ToPascalCase(match.Groups["level"].Value)),

                        EquipmentNumber: int.Parse(match.Groups["equipment"].Value),
                        ConveyorLineNumber: int.Parse(match.Groups["line"].Value),
                        PrinterNumber: int.Parse(match.Groups["printer"].Value),
                        PrinterStatus: bool.Parse(match.Groups["status"].Value),
                        TagName: match.Groups["tag"].Value,
                        ScannerNumber: int.Parse(match.Groups["scanner"].Value),
                        TagIndex: int.Parse(match.Groups["index"].Value)
                    );

                return true;
            }
        }

        #endregion

    }
}
