namespace LogParser.Devices.Enums;

internal enum MessageLevel : byte
{
    None = 0x00,
    Info = 0x01,
    Debug = 0x02,
    Warning = 0x04,
    Error = 0x08
}