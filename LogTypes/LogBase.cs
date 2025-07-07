using LogParser.Systems.Model;
using LogParser.Systems.ViewModel;

namespace LogParser.LogTypes
{
    internal abstract class LogBase<TModel, TViewModel>(
            TViewModel viewModel,
            LogReaderConsole? console = null)
        where TModel : SystemModelBase
        where TViewModel : SystemViewModelBase<TModel>
    {
        #region Fields

        protected readonly LogReaderConsole? _console = console;

        protected readonly List<TModel> _inboundHistory = [];

        #endregion

        #region Properties

        public DateTimeOffset? LogTimeStamp { get; set; }

        public TViewModel ViewModel { get; } = viewModel;

        #endregion

        #region Methods

        public abstract void ProcessLine(string line);

        #endregion
    }
}