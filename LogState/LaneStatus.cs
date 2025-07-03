namespace LogParser.LogState
{
    internal enum LaneStatus : byte
    {
        None = 0x00,
        Disabled = 0x01,
        Full = 0x02,
        PartiallyFull = 0x04
    }
}