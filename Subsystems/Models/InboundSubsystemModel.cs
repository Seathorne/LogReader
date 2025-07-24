using LogParser.Devices.Models;
using LogParser.Subsystems.Interface;
using System.Collections.Immutable;

namespace LogParser.Subsystems.Models;

internal record InboundSubsystemModel(
    IImmutableSet<PrinterModel> Printers,
    IImmutableSet<ScannerModel> Scanners) : ISubsystemModel
{
    #region Methods

    public static InboundSubsystemModel FromSets(ISet<PrinterModel> printers, ISet<ScannerModel> scanners)
    {
        return new InboundSubsystemModel(printers.ToImmutableHashSet(), scanners.ToImmutableHashSet());
    }

    #endregion
}