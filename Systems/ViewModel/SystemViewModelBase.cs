using LogParser.Devices.ViewModel;
using LogParser.Systems.Model;

namespace LogParser.Systems.ViewModel
{
    internal abstract class SystemViewModelBase<TModel>(TModel initialModel) : RecordViewModelBase<TModel>(initialModel) where TModel : SystemModelBase
    {
        public DateTimeOffset? TimeStamp
        {
            get => Model.TimeStamp;
            set => UpdateModel(model => model with { TimeStamp = value });
        }
    }
}