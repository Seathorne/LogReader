using LogParser.Common.Structs;
using LogParser.Common.Tracking;
using LogParser.Devices.Enums;
using LogParser.Devices.Models;
using System.Net;

namespace LogParser.Devices.Tracking;

internal class TrackedPrinter : HistoryTracker<PrinterModel>
{
    #region Properties

    public int PrinterID
    {
        get => Model.PrinterID;
        set => UpdateModel(m => m with { PrinterID = value });
    }

    public IPAddress IPAddress
    {
        get => Model.IPAddress;
        set => UpdateModel(m => m with { IPAddress = value });
    }

    public PrinterStatus? Status
    {
        get => Model.Status;
        set => UpdateModel(m => m with { Status = value });
    }

    public TimeOnly? LastUsedTime
    {
        get => Model.LastUsedTime;
        set => UpdateModel(m => m with { LastUsedTime = value });
    }

    #endregion

    #region Constructors

    public TrackedPrinter()
    {
    }

    public TrackedPrinter(
            Timestamp creationTime,
            int printerID,
            IPAddress ipAddress,
            PrinterStatus? status = null,
            TimeOnly? lastUsedTime = null)
        : base(creationTime, new PrinterModel(printerID, ipAddress, status, lastUsedTime))
    {
    }

    #endregion

    #region Methods

    public void UpdateWithTimestamp(
        Timestamp updateTime,
        int printerID,
        IPAddress iPAddress,
        PrinterStatus? status = null,
        TimeOnly? lastUsedTime = null)
    {
        using (WithTimestamp(updateTime))
        {
            UpdateModel(m => m with
            {
                PrinterID = printerID,
                IPAddress = iPAddress,
                Status = status,
                LastUsedTime = lastUsedTime
            });
        }
    }

    #endregion
}