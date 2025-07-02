using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LogReader.Messages
{
    internal readonly struct PrinterStatusUpdateMessage(DateTime logTimeStamp, int hour, int minutes, int seconds, int milliseconds, int eventHour, int eventMinutes, int eventSeconds,
        int eventMilliseconds, int threadNumber, MessageLevel messageLevel, int equipmentNumber, int conveyorLineNumber, int printerNumber, bool printerStatus, string tagName, int scannerNumber, int tagIndex)
    {
        [StringSyntax(StringSyntaxAttribute.Regex)]
        private static readonly string TimeStampPattern = @"(?<time>(?<hour>\d{2}):(?<minutes>\d{2}):(?<seconds>\d{2})\.(?<milliseconds>\d{3}))";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private static readonly string EventTimeStampPattern = @"(?<_time>(?<_hour>\d{2}):(?<_minutes>\d{2}):(?<_seconds>\d{2})\.(?<_milliseconds>\d{3}))";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private static readonly string ThreadNumberPattern = @"(?<thread>\d+)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private static readonly string MessageLevelPattern = @"(?<level>INFO|DEBUG|WARNING|ERROR)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private static readonly string EquipmentNumberPattern = @"Equipment (?<equipment>\d+)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private static readonly string PrinterStatusPattern = @"Update Printer Receiving Line (?<line>\d) PandA (?<printer>\d) Enabled (?<status>True|False) using Tag: (?<tag>SCANNER_(?<scanner>\d+)PRINTER_W_STATUS\[(?<index>(\d+)\]))";
        
        [StringSyntax(StringSyntaxAttribute.Regex)]
        private static readonly string TagPattern = @"using Tag: (?<tag>SCANNER_(?<scanner>\d+)PRINTER_W_STATUS\[(?<index>(\d+)\]))";

        public DateTimeOffset TimeStamp { get; } = new DateTimeOffset
            (
                year: logTimeStamp.Year,
                month: logTimeStamp.Month,
                day: logTimeStamp.Day,
                hour: hour,
                minute: minutes,
                second: seconds,
                millisecond: milliseconds,
                offset: Configuration.TimeZone.GetUtcOffset(logTimeStamp)
            );

        public DateTimeOffset EventTimeStamp { get; } = new DateTimeOffset
            (
                year: logTimeStamp.Year,
                month: logTimeStamp.Month,
                day: logTimeStamp.Day,
                hour: eventHour,
                minute: eventMinutes,
                second: eventSeconds,
                millisecond: eventMilliseconds,
                offset: Configuration.TimeZone.GetUtcOffset(logTimeStamp)
            );

        public int ThreadNumber { get; } = threadNumber;

        public MessageLevel MessageLevel { get; } = messageLevel;

        public int EquipmentNumber { get; } = equipmentNumber;

        public int ConveyorLineNumber { get; } = conveyorLineNumber;

        public int PrinterNumber { get; } = printerNumber;

        public bool PrinterStatus { get; } = printerStatus;

        public string TagName { get; } = tagName;

        public int ScannerNumber { get; } = scannerNumber;

        public int TagIndex { get; } = tagIndex;

        public readonly string PrinterName => $"Receiving Line {ConveyorLineNumber} PandA {PrinterNumber}";

        public static bool TryParse(string message, DateTime logTimeStamp, out PrinterStatusUpdateMessage result)
        {
            var pattern = string.Format("{0} [{1}] (null) {2}  {3} - {4} - {5} {6}",
                TimeStampPattern,
                ThreadNumberPattern,
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
                        logTimeStamp: logTimeStamp,
                        hour: int.Parse(match.Groups["hour"].Value),
                        minutes: int.Parse(match.Groups["minutes"].Value),
                        seconds: int.Parse(match.Groups["seconds"].Value),
                        milliseconds: int.Parse(match.Groups["milliseconds"].Value),

                        eventHour: int.Parse(match.Groups["_hour"].Value),
                        eventMinutes: int.Parse(match.Groups["_minutes"].Value),
                        eventSeconds: int.Parse(match.Groups["_seconds"].Value),
                        eventMilliseconds: int.Parse(match.Groups["_milliseconds"].Value),

                        threadNumber: int.Parse(match.Groups["thread"].Value),
                        messageLevel: Enum.Parse<MessageLevel>(match.Groups["level"].Value),

                        equipmentNumber: int.Parse(match.Groups["equipment"].Value),
                        conveyorLineNumber: int.Parse(match.Groups["line"].Value),
                        printerNumber: int.Parse(match.Groups["printer"].Value),
                        printerStatus: bool.Parse(match.Groups["status"].Value),
                        tagName: match.Groups["tag"].Value,
                        scannerNumber: int.Parse(match.Groups["scanner"].Value),
                        tagIndex: int.Parse(match.Groups["index"].Value)
                    );

                return true;
            }
        }
    }
}
