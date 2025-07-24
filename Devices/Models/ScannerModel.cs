using LogParser.Common.Records;
using LogParser.Devices.Enums;
using LogParser.Devices.Interface;
using System.Collections.Immutable;

namespace LogParser.Devices.Models;

internal record ScannerModel(
    ScannerName ScannerName,
    IImmutableQueue<Container> ScannedContainers) : IDeviceModel
{
    #region Constructors

    public ScannerModel(ScannerName scannerName, params Container[] scannedContainers)
        : this(scannerName, ImmutableQueue.Create(scannedContainers))
    {
    }

    #endregion
}