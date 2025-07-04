namespace LogParser.Devices.ViewModel
{
    internal abstract class RecordViewModelBase<TModel>(TModel initialModel) : ViewModelBase where TModel : class
    {
        private TModel _model = initialModel;

        public event Action<TModel, TModel>? ModelChanged;
        
        public event Action<TModel>? ModelUpdated;

        internal event Action? RequestSystemUpdate;

        internal TModel Model
        {
            get => _model;
            set
            {
                if (!ReferenceEquals(_model, value))
                {
                    var oldModel = _model;
                    _model = value;

                    foreach (var prop in GetChangedProperties(oldModel, value))
                    {
                        OnPropertyChanged(prop);
                    }

                    OnPropertyChanged(nameof(Model));
                    ModelChanged?.Invoke(oldModel, value);
                }
                ModelUpdated?.Invoke(value);
            }
        }

        public void UpdateModel(TModel newModel)
        {
            Model = newModel;
            RequestSystemUpdate?.Invoke();
        }

        protected void UpdateModel(Func<TModel, TModel> update)
        {
            Model = update(Model);
            RequestSystemUpdate?.Invoke();
        }
    }
}