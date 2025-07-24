using LogParser.Common.Exceptions;
using LogParser.Common.Structs;
using System.Collections.Concurrent;
using System.Reflection;

namespace LogParser.Common.Tracking;

internal abstract class HistoryTracker<TModel>
{
    #region Fields

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = [];

    private readonly List<HistoryDelta> _history = [];

    private readonly List<Checkpoint<TModel>> _checkpoints = [];

    private Timestamp? _timeStampContext;

    #endregion

    #region Events

    public event Action<TModel, TModel, Timestamp>? ModelChanged;

    #endregion

    #region Constructors

    protected HistoryTracker()
    {
    }

    protected HistoryTracker(Timestamp creationTime, TModel initialModel)
    {
        using (WithTimestamp(creationTime))
        {
            UpdateModel(initialModel);
        }
    }

    #endregion

    #region Properties

    internal TModel Model
    {
        get => field ?? throw new UninitializedException(nameof(Model));
        private set
        {
            if (!ReferenceEquals(field, value))
            {
                var oldModel = field;
                field = value;

                if (oldModel is not null)
                {
                    ModelChanged?.Invoke(oldModel, field, _timeStampContext ?? throw new InvalidOperationException("Timestamp context must be used before updating record."));
                    RecordHistory(oldModel, field);
                }
                else StartHistoryTracking(field);
            }
        }
    }

    internal int ModelSnapshotFrequency { get; set; } = 2;

    #endregion

    #region Public Methods

    public IDisposable WithTimestamp(Timestamp timestamp)
    {
        _timeStampContext = timestamp;
        return new TimestampScope(cleanup: () => _timeStampContext = null);
    }

    public void UpdateWithTimestamp(Timestamp timestamp, TModel newModel)
    {
        using (WithTimestamp(timestamp))
        {
            UpdateModel(newModel);
        }
    }

    public TModel GetSnapshot(Timestamp timeStamp)
    {
        var properties = _propertyCache.GetOrAdd(
            key: GetType(),
            valueFactory: type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
        );

        // Get last checkpoint at/before timestamp
        var checkpoint = _checkpoints.Last(x => x.TimeStamp <= timeStamp);
        if (checkpoint.TimeStamp == timeStamp)
        {
            return checkpoint.Model;
        }

        var model = checkpoint.Model;

        // Naive "fast-forward from last checkpoint" implementation
        int startIndex = _history.FindIndex(x => x.Timestamp > timeStamp);
        for (int i = 0; i < _history.Count; i++)
        {
            var entry = _history[i];
            if (entry.Timestamp > timeStamp)
                break;

            foreach (var pair in entry.ChangedProperties)
            {
                typeof(TModel).GetProperty(pair.Key)?.SetValue(model, pair.Value);
            }
        }
        return model;
    }

    #endregion

    #region Methods

    protected void UpdateModel(TModel newModel)
    {
        // Property setter handles events and history tracking
        Model = newModel;
    }

    protected void UpdateModel(Func<TModel, TModel> update)
    {
        // Property setter handles events and history tracking
        Model = update(Model);
    }

    private void StartHistoryTracking(TModel initialModel)
    {
        // Access cached reflection results
        var properties = _propertyCache.GetOrAdd(
            key: Model?.GetType() ?? throw new UninitializedException("Model must be initialized with a non-null value"),
            valueFactory: type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
        );

        var changedProperties = properties
            .Select(prop => new
            {
                Property = prop,
                NewValue = prop.GetValue(initialModel)
            })
            .Where(x => !Equals(null, x.NewValue))
            .ToDictionary(
                keySelector: x => x.Property.Name,
                elementSelector: x => x.NewValue
            );

        Timestamp timeStamp = _timeStampContext ?? throw new InvalidOperationException($"Timestamp context must be used before updating record.");
        _history.Add(new HistoryDelta(timeStamp, changedProperties));
        _checkpoints.Add(new Checkpoint<TModel>(timeStamp, initialModel));
    }

    private void RecordHistory(TModel oldModel, TModel newModel)
    {
        // Access cached reflection results
        var properties = _propertyCache.GetOrAdd(
            key: Model?.GetType() ?? throw new UninitializedException("Model must be initialized with a non-null value"),
            valueFactory: type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
        );

        var changedProperties = properties
            .Select(prop => new
            {
                Property = prop,
                OldValue = prop.GetValue(oldModel),
                NewValue = prop.GetValue(newModel)
            })
            .Where(x => !Equals(x.OldValue, x.NewValue))
            .ToDictionary(
                keySelector: x => x.Property.Name,
                elementSelector: x => x.NewValue
            );

        Timestamp timeStamp = _timeStampContext ?? throw new InvalidOperationException($"TimeStamp context must be used before updating record.");
        _history.Add(new HistoryDelta(timeStamp, changedProperties));
        if ((_history.Count + 1) % ModelSnapshotFrequency == 0)
        {
            _checkpoints.Add(new Checkpoint<TModel>(timeStamp, newModel));
        }
    }

    #endregion

    #region Nested Classes

    private class TimestampScope(Action cleanup) : IDisposable
    {
        private readonly Action _cleanup = cleanup;

        public void Dispose() => _cleanup();
    }

    #endregion
}