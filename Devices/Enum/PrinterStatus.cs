namespace LogParser.Devices.Enum
{
    internal enum PrinterStatus : byte
    {
        Off = 0x00,
        Enabled = 0x01,
        Active = 0x02,
        On = Enabled | Active
    }
}
