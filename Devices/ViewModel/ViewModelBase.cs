using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LogParser.Devices.ViewModel
{
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void UpdateModel<TModel>(
            ref TModel model,
            Func<TModel, TModel> update,
            Action<TModel, TModel>? onModelChanged = null,
            Action<TModel>? onModelUpdated = null) where TModel : class
        {
            var oldModel = model;
            var newModel = update(oldModel);

            if (!ReferenceEquals(oldModel, newModel))
            {
                model = newModel;

                // Get changed properties efficiently
                var changedProps = GetChangedProperties(oldModel, newModel).ToList();

                // Batch notifications
                foreach (var prop in changedProps)
                {
                    OnPropertyChanged(prop);
                }

                OnPropertyChanged(nameof(Model));
                onModelChanged?.Invoke(oldModel, newModel);
            }

            onModelUpdated?.Invoke(newModel);
        }

        protected static IEnumerable<string> GetChangedProperties<TModel>(TModel oldModel, TModel newModel)
        {
            if (oldModel == null || newModel == null)
                yield break;

            // Cache reflection results
            var properties = PropertyCache.GetOrAdd(
                typeof(TModel),
                t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            );

            foreach (var prop in properties)
            {
                var oldValue = prop.GetValue(oldModel);
                var newValue = prop.GetValue(newModel);

                if (!Equals(oldValue, newValue))
                {
                    yield return prop.Name;
                }
            }
        }
    }
}