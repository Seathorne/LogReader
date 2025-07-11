using LogParser.Systems.Model;
using LogParser.Systems.ViewModel;
using LogParser.Utility;

namespace LogParser.LogTypes
{
    internal abstract class LogBase<TModel, TViewModel>(
            TViewModel viewModel,
            (Type MessageType, bool IsEnabled)[]? enabledMessages = null,
            LogReaderConsole? console = null)
        where TModel : SystemModelBase, new()
        where TViewModel : SystemViewModelBase<TModel>, new()
    {
        #region Fields

        protected readonly LogReaderConsole? _console = console;

        protected readonly List<TModel> _inboundHistory = [];

        #endregion

        #region Properties

        public DateTimeOffset? LogTimeStamp { get; set; }

        public TViewModel ViewModel { get; } = viewModel;

        public ObservableDictionary<Type, bool> EnabledMessages { get; } = enabledMessages?.ToObservableDictionary(x => x.MessageType, x => x.IsEnabled) ?? [];

        #endregion

        #region Methods

        public abstract void ProcessLine(string line);

        #endregion
    }
}