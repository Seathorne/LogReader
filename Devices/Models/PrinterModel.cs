using LogParser.Devices.Enums;
using LogParser.Devices.Interface;
using System.Net;

namespace LogParser.Devices.Models;

internal record PrinterModel(
    int PrinterID,
    IPAddress IPAddress,
    PrinterStatus? Status = null,
    TimeOnly? LastUsedTime = null
) : IDeviceModel;