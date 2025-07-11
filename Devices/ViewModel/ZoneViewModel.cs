using LogParser.Devices.Model;

namespace LogParser.Devices.ViewModel
{
    internal class ZoneViewModel : RecordViewModelBase<ZoneModel>
    {
        public string? ZoneId
        {
            get => Model.ZoneID;
            set => UpdateModel(m => m with { ZoneID = value });
        }

        public ZoneViewModel()
        {
        }

        public ZoneViewModel(string zoneID) : base(new ZoneModel(zoneID))
        {
        }
    }
}