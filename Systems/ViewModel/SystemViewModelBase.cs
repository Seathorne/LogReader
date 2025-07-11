using LogParser.Devices.ViewModel;
using LogParser.Systems.Model;

namespace LogParser.Systems.ViewModel
{
    internal abstract class SystemViewModelBase<TModel> : RecordViewModelBase<TModel> where TModel : SystemModelBase, new()
    {
        public SystemViewModelBase()
        {
        }

        public SystemViewModelBase(TModel initialModel) : base(initialModel)
        {
        }
    }
}