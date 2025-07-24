using LogParser.Common.Records;
using LogParser.Common.Structs;
using LogParser.Common.Tracking;
using LogParser.Devices.Enums;
using LogParser.Devices.Models;
using System.Collections.Immutable;

namespace LogParser.Devices.Tracking;

internal class TrackedScanner : HistoryTracker<ScannerModel>
{
    #region

    public ScannerName ScannerName
    {
        get => Model.ScannerName;
        set => UpdateModel(m => m with { ScannerName = value });
    }

    public IImmutableQueue<Container> ScannedContainers
    {
        get => Model.ScannedContainers;
        set => UpdateModel(m => m with { ScannedContainers = value });
    }

    #endregion

    #region Constructors

    public TrackedScanner()
    {
    }

    public TrackedScanner(Timestamp creationTime, ScannerName scannerName, params Container[] containers)
        : base(creationTime, new ScannerModel(scannerName, ImmutableQueue.Create(containers)))
    {
    }

    #endregion
}