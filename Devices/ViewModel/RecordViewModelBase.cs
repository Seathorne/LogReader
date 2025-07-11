using LogParser.Devices.Model;

namespace LogParser.Devices.ViewModel
{
    internal abstract class RecordViewModelBase<TModel> : ViewModelBase where TModel : RecordModelBase, new()
    {
        #region Fields

        private TModel _model;

        #endregion

        #region Events

        public event Action<TModel, TModel>? ModelChanged;

        #endregion

        #region Constructors

        protected RecordViewModelBase()
        {
            _model = new TModel();
        }

        protected RecordViewModelBase(TModel initialModel)
        {
            _model = initialModel;
        }

        #endregion

        #region Properties

        internal TModel Model
        {
            get => _model;
            private set
            {
                if (!ReferenceEquals(_model, value))
                {
                    var oldModel = _model;
                    _model = value;

                    OnPropertiesChanged(oldModel, value);
                    OnPropertyChanged(nameof(Model));
                    ModelChanged?.Invoke(oldModel, value);
                }
            }
        }

        #endregion

        #region Methods

        public void UpdateModel(TModel newModel)
        {
            Model = newModel;
        }

        protected void UpdateModel(Func<TModel, TModel> update)
        {
            Model = update(Model);
        }

        #endregion
    }
}