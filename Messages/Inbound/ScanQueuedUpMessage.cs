using LogParser.Devices.Enum;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LogParser.Messages
{
    internal partial record ScanQueuedUpMessage(DateTimeOffset TimeStamp, DateTimeOffset EventTimeStamp,
        int ThreadNumber, MessageLevel MessageLevel, int EquipmentNumber, ImmutableArray<string> Barcodes, int ConveyorLineNumber, ScannerName ScannerType) : MessageBase
    {
        #region Constants

        public const string MessagePattern = $"{TimeStampPattern} {ThreadNumberPattern} {ThreadNullPattern} {MessageLevelPattern}  {EquipmentNumberPattern} - {EventTimeStampPattern} - {BarcodePattern} {ScannerPattern}";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string BarcodePattern = @"SCAN (?<barcodes>(?:(?<barcode>\w*\-?\d+),?\s?)*)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string ScannerPattern = @"Queued UP For Scanner Line (?<line>1|2) (?<scanner>Receiving|Verification) Scanner TIME START";

        #endregion

        #region Properties

        public static ScanQueuedUpMessage DefaultRecord => new(DateTimeOffset.MinValue, DateTimeOffset.MinValue, -1, MessageLevel.None, -1, [], -1, Devices.Enum.ScannerName.None);

        [GeneratedRegex(MessagePattern)]
        private static partial Regex ScanQueuedUpRegex { get; }

        public string ScannerName => $"Line {ConveyorLineNumber} {ScannerType} Scanner";

        #endregion

        #region Methods

        public static bool TryParse(string message, DateTimeOffset logTimeStamp, out ScanQueuedUpMessage result)
        {
            var match = ScanQueuedUpRegex.Match(message);

            if (!match.Success)
            {
                result = DefaultRecord;
                return false;
            }
            else
            {
                var barcodes = match.Groups["barcode"].Captures.Select(capture => capture.Value).ToArray();

                result = new ScanQueuedUpMessage
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
                        Barcodes: ImmutableArray.Create(barcodes),
                        ConveyorLineNumber: int.Parse(match.Groups["line"].Value),
                        ScannerType: Enum.Parse<ScannerName>(match.Groups["scanner"].Value)
                    );

                return true;
            }
        }

        #endregion
    }
}