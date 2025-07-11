using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LogParser.Devices.ViewModel
{
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Methods

        protected static IEnumerable<string> GetChangedProperties<T>(T oldModel, T newModel)
        {
            if (oldModel == null || newModel == null)
                yield break;

            // Cache reflection results
            var properties = _propertyCache.GetOrAdd(
                key: typeof(T),
                valueFactory: type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
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

        protected void OnPropertiesChanged<T>(T oldModel, T newModel)
        {
            // Get changed properties efficiently
            var changedProps = GetChangedProperties(oldModel, newModel).ToList();

            // Batch notifications
            foreach (var prop in changedProps)
            {
                OnPropertyChanged(prop);
            }
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}