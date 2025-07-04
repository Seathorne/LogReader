using LogParser.Devices.Model;
using LogParser.Devices.Enum;

namespace LogParser.Devices.ViewModel
{
    internal class LaneViewModel : RecordViewModelBase<LaneModel>
    {
        public int LaneNumber => Model.LaneNumber;

        public LaneStatus? Status
        {
            get => Model.Status;
            set => UpdateModel(printer => printer with { Status = value });
        }

        public LaneViewModel(int id, LaneStatus? status = null) : base(new LaneModel(id, status))
        {
            Model = new LaneModel(id, status);
        }
    }
}