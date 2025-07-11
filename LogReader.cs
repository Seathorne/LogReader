using LogParser.Devices.Enum;
using LogParser.Devices.ViewModel;
using LogParser.LogTypes;
using LogParser.Messages;
using LogParser.Messages.Inbound;
using LogParser.Systems.ViewModel;
using System.Net;

namespace LogParser
{
    public class LogReader : IDisposable
    {
        #region Fields

        private FileStream? fileStream;
        
        private StreamReader? streamReader;

        private readonly InboundLog _inboundLog;

        #endregion

        #region Properties

        public required TimeZoneInfo TimeZone { get; set; }

        public LogReaderConsole Console { get; } = new();

        #endregion

        public LogReader()
        {
            _inboundLog = new InboundLog(
                viewModel: new InboundSystemViewModel(
                    printers: new HashSet<PrinterViewModel> {
                        new(printerID: 1, ipAddress: new IPAddress([172, 24, 18, 38])),
                        new(printerID: 2, ipAddress: new IPAddress([172, 24, 18, 39])),
                        new(printerID: 3, ipAddress: new IPAddress([172, 24, 18, 40])),
                        new(printerID: 4, ipAddress: new IPAddress([172, 24, 18, 41]))
                    },
                    zones: new HashSet<ZoneViewModel> {
                        new(zoneID: "000")
                    },
                    queuedContainers: new Dictionary<ScannerName, Queue<ContainerViewModel>> {
                        { ScannerName.Receiving, [] },
                        { ScannerName.Verification, [] }
                    }
                ),
                enabledMessages: [
                    (typeof(PrinterStatusUpdate), true),
                    (typeof(LaneStatusUpdate), false),
                    (typeof(ZonesFoundMessage), false),
                    (typeof(ScanQueuedUpMessage), true)
                ],
                console: Console);
        }

        public void Dispose()
        {
            fileStream?.Dispose();
            streamReader?.Dispose();
            GC.SuppressFinalize(this);

            fileStream = null;
            streamReader = null;
        }

        public void ProcessFile(string filePath)
        {
            fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            streamReader = new StreamReader(fileStream);

            var writeTime = File.GetLastWriteTime(filePath);
            var writeTimeOffset = new DateTimeOffset(writeTime, TimeZone.GetUtcOffset(writeTime));
            _inboundLog.LogTimeStamp = writeTimeOffset;

            string[] lines = streamReader.ReadToEnd().Split(Environment.NewLine);
            foreach (string line in lines)
            {
                _inboundLog.ProcessLine(line);
            }
        }
    }
}