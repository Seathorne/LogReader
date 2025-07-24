using LogParser.Common.Structs;
using LogParser.Devices.Enums;
using LogParser.Devices.Tracking;
using LogParser.Equipment.Common.Enums;
using LogParser.Equipment.Inbound.Logs;
using LogParser.Subsystems.Tracking;
using LogParser.System;
using LogParser.System.Exceptions;
using System.Net;
using System.Text.RegularExpressions;

namespace LogParser
{
    public partial class LogReader : IDisposable
    {
        #region Fields

        private FileStream? _fileStream;
        
        private StreamReader? _streamReader;

        private readonly InboundLog _inboundLog;

        private readonly HashSet<TrackedPrinter> _printers = [];

        private readonly HashSet<TrackedScanner> _scanners = [];

        private readonly TrackedInboundSubsystem _inboundSubsystem = new();

        private readonly Dictionary<MessageClass, bool> _enabledMessages;

        #endregion

        #region Properties

        public required TimeZoneInfo TimeZone { get; set; }

        public LogReaderConsole Console { get; } = new();

        public bool IsFileLoaded { get; private set; }

        public string? LoadedFilePath { get; private set; }

        [GeneratedRegex(@"^(?<time>(?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})\.(?<millisecond>\d{3}))")]
        private static partial Regex TimestampPattern { get; }

        #endregion

        #region Events

        public event Action<string>? WriteBack;

        #endregion

        #region Constructors

        public LogReader()
        {
            Console.OnWriteLine += (o, e) => WriteBack?.Invoke(e.Line);

            _enabledMessages = new Dictionary<MessageClass, bool>
            {
                [MessageClass.PrinterStatusUpdateMessage] = true,
                [MessageClass.LaneStatusUpdateMessage] = false,
                [MessageClass.ZonesFoundMessage] = false,
                [MessageClass.ContainerScannedMessage] = true
            };
            _inboundLog = new InboundLog(_inboundSubsystem, _enabledMessages, Console);
        }

        #endregion

        #region Public Methods

        public void OpenFile(string path)
        {
            if (IsFileLoaded)
                throw new InvalidOperationException("A file is already open.");

            _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            _streamReader = new StreamReader(_fileStream);
            LoadedFilePath = path;
            IsFileLoaded = true;
        }

        public void CloseFile()
        {
            if (IsFileLoaded)
            {
                _fileStream?.Dispose();
                _streamReader?.Dispose();

                _fileStream = null;
                _streamReader = null;
                LoadedFilePath = null;
                IsFileLoaded = false;
            }
        }

        public void ProcessFile()
        {
            ProcessThrough(int.MaxValue);
        }

        public void ProcessThrough(int lineNumber)
        {
            if (!IsFileLoaded || _fileStream is null || _streamReader is null || LoadedFilePath is null)
                throw new InvalidOperationException("File must be loaded before processing.");

            if (lineNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(lineNumber), lineNumber, "Line number must be greater than 0.");

            var writeTime = File.GetLastWriteTime(LoadedFilePath);
            var writeTimeOffset = new DateTimeOffset(writeTime, TimeZone.GetUtcOffset(writeTime));
            _inboundLog.LogTimeStamp = writeTimeOffset;

            string? firstLine = _streamReader.ReadLine();
            if (firstLine is null)
                return;

            var match = TimestampPattern.Match(firstLine);
            if (match.Success != true)
                throw new FileParseException("File could not be parsed.");

            var groups = match.Groups;

            var initialTime = new DateTimeOffset(
                year: writeTimeOffset.Year,
                month: writeTimeOffset.Month,
                day: writeTimeOffset.Day,
                hour: int.Parse(groups["hour"].Value),
                minute: int.Parse(groups["minute"].Value),
                second: int.Parse(groups["second"].Value),
                millisecond: int.Parse(groups["millisecond"].Value),
                offset: writeTimeOffset.Offset
            );

            InitializeModels(new Timestamp(initialTime, lineNumber));

            for (int i = 2; i <= lineNumber; i++)
            {
                string? line = _streamReader.ReadLine();
                
                if (line is null)
                    break;

                _inboundLog.ProcessLine(line, i);
            }
        }

        public void Dispose()
        {
            CloseFile();
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        private void InitializeModels(Timestamp initialTime)
        {
            _printers.Add(new TrackedPrinter(initialTime, printerID: 1, ipAddress: new IPAddress([172, 24, 18, 38])));
            _printers.Add(new TrackedPrinter(initialTime, printerID: 2, ipAddress: new IPAddress([172, 24, 18, 39])));
            _printers.Add(new TrackedPrinter(initialTime, printerID: 3, ipAddress: new IPAddress([172, 24, 18, 40])));
            _printers.Add(new TrackedPrinter(initialTime, printerID: 4, ipAddress: new IPAddress([172, 24, 18, 41])));

            _scanners.Add(new TrackedScanner(initialTime, ScannerName.Verification));
            _scanners.Add(new TrackedScanner(initialTime, ScannerName.Receiving));

            _inboundSubsystem.UpdateWithTimestamp(initialTime, _printers, _scanners);
        }

        #endregion
    }
}