using LogParser.Devices.Model;

namespace LogParser.Devices.ViewModel
{
    internal class ZoneViewModel : RecordViewModelBase<ZoneModel>
    {
        public string ZoneId => Model.ZoneId;

        public ZoneViewModel(string id) : base(new ZoneModel(id))
        {
            Model = new ZoneModel(id);
        }
    }
}