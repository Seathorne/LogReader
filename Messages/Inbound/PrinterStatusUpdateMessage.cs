using LogParser.Devices.Enum;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LogParser.Messages
{
    internal partial record PrinterStatusUpdateMessage(DateTimeOffset TimeStamp, DateTimeOffset EventTimeStamp,
        int ThreadNumber, MessageLevel MessageLevel, int EquipmentNumber, int ConveyorLineNumber, int PrinterId, PrinterStatus PrinterStatus, int ScannerNumber, string TagName, int TagIndex) : MessageBase
    {
        #region Constants

        public const string MessagePattern = $"{TimeStampPattern} {ThreadNumberPattern} {ThreadNullPattern} {MessageLevelPattern}  {EquipmentNumberPattern} - {EventTimeStampPattern} - {PrinterStatusPattern} {TagPattern}";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string PrinterStatusPattern = @"Update Printer Receiving Line (?<line>1|2) PandA (?<printer>\d) Enabled (?<status>True|False)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string TagPattern = @"using Tag: (?<tag>SCANNER_(?<scanner>\d+)_PRINTER_W_STATUS\[(?<index>\d+)\])";

        #endregion

        #region Properties

        public static PrinterStatusUpdateMessage DefaultRecord => new(DateTimeOffset.MinValue, DateTimeOffset.MinValue, -1, MessageLevel.None, -1, -1, -1, PrinterStatus.Off, -1, "", -1);

        [GeneratedRegex(MessagePattern)]
        private static partial Regex PrinterStatusUpdateRegex { get; }

        public string PrinterName => $"PandA {PrinterId} (Receiving Line {ConveyorLineNumber})";

        #endregion

        #region Methods

        public static bool TryParse(string message, DateTimeOffset logTimeStamp, out PrinterStatusUpdateMessage result)
        {
            var match = PrinterStatusUpdateRegex.Match(message);

            if (!match.Success)
            {
                result = DefaultRecord;
                return false;
            }
            else
            {
                result = new PrinterStatusUpdateMessage
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

                        EventTimeStamp: new DateTimeOffset
                            (
                                year: logTimeStamp.Year,
                                month: logTimeStamp.Month,
                                day: logTimeStamp.Day,
                                hour: int.Parse(match.Groups["_hour"].Value),
                                minute: int.Parse(match.Groups["_minute"].Value),
                                second: int.Parse(match.Groups["_second"].Value),
                                millisecond: int.Parse(match.Groups["_millisecond"].Value),
                                offset: logTimeStamp.Offset
                            ),

                        ThreadNumber: int.Parse(match.Groups["thread"].Value),
                        MessageLevel: Enum.Parse<MessageLevel>(ToPascalCase(match.Groups["level"].Value)),

                        EquipmentNumber: int.Parse(match.Groups["equipment"].Value),
                        ConveyorLineNumber: int.Parse(match.Groups["line"].Value),
                        PrinterId: int.Parse(match.Groups["printer"].Value),
                        PrinterStatus: bool.Parse(match.Groups["status"].Value) ? PrinterStatus.Enabled : PrinterStatus.Off,
                        ScannerNumber: int.Parse(match.Groups["scanner"].Value),

                        TagName: match.Groups["tag"].Value,
                        TagIndex: int.Parse(match.Groups["index"].Value)
                    );

                return true;
            }
        }

        #endregion

    }
}
