using LogParser.Devices.Enum;
using LogParser.Devices.ViewModel;
using LogParser.LogTypes;
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
                    printers: [
                        new PrinterViewModel(id: 1, ipAddress: new IPAddress([172, 24, 18, 38])),
                        new PrinterViewModel(id: 2, ipAddress: new IPAddress([172, 24, 18, 39])),
                        new PrinterViewModel(id: 3, ipAddress: new IPAddress([172, 24, 18, 40])),
                        new PrinterViewModel(id: 4, ipAddress: new IPAddress([172, 24, 18, 41]))
                    ],
                    zones: [
                        new ZoneViewModel(id: "000")
                    ],
                    queuedContainers: [
                        (ScannerName.Receiving, new List<ContainerViewModel>()),
                        (ScannerName.Verification, new List<ContainerViewModel>())
                    ]
                ),
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