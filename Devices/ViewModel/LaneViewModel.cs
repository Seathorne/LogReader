using LogParser.Devices.Model;
using LogParser.Devices.Enum;

namespace LogParser.Devices.ViewModel
{
    internal class LaneViewModel : RecordViewModelBase<LaneModel>
    {
        public int? LaneID {
            get => Model.LaneID;
            set => UpdateModel(m => m with { LaneID = value });
        }

        public LaneStatus? Status
        {
            get => Model.Status;
            set => UpdateModel(m => m with { Status = value });
        }

        public LaneViewModel(int laneID, LaneStatus? status = null) : base(new LaneModel(laneID, status))
        {
        }
    }
}