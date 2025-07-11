namespace LogParser.Devices.Model
{
    internal record ZoneModel(string? ZoneID = null)
        : RecordModelBase
    {
        public ZoneModel() : this(ZoneID: null)
        {
        }
    }
}