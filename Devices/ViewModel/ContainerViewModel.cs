using LogParser.Devices.Model;
using LogParser.Devices.Enum;

namespace LogParser.Devices.ViewModel
{
    internal class ContainerViewModel : RecordViewModelBase<ContainerModel>
    {
        public ContainerType ContainerType
        {
            get => Model.ContainerType;
            set => UpdateModel(cntnr => cntnr with { ContainerType = value });
        }

        public string? LPN
        {
            get => Model.LPN;
            set => UpdateModel(cntnr => cntnr with { LPN = value });
        }

        public string? LotNumber
        {
            get => Model.LotNumber;
            set => UpdateModel(cntnr => cntnr with { LotNumber = value });
        }

        public ContainerViewModel(params string[] barcodes) : base(new ContainerModel(barcodes))
        {
        }

        public ContainerViewModel(ContainerType containerType, string? lpn, string? lotNumber = null)
            : base(new ContainerModel(containerType, lpn, lotNumber))
        {
        }

        public void Update(ContainerType containerType, string? lpn = null, string? lotNumber = null)
        {
            Model = new ContainerModel(containerType, lpn, lotNumber);
        }
    }
}