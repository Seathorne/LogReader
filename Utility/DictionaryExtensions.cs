﻿using System.Collections.ObjectModel;

namespace LogParser.Utility
{
    internal static class DictionaryExtensions
    {
        //public static ILookup<TKey, TValue> ToLookup<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary)
        //        where TKey : notnull =>
        //    dictionary.SelectMany(kvp => kvp.Value.Select(value => new { kvp.Key, Value = value }))
        //              .ToLookup(entry => entry.Key, x => x.Value);

        public static ILookup<TKey, TValue> ToLookup<TKey, TValue>(this ObservableDictionary<TKey, ObservableCollection<TValue>> dictionary)
                where TKey : notnull =>
            dictionary.SelectMany(kvp => kvp.Value.Select(value => new { kvp.Key, Value = value }))
                      .ToLookup(entry => entry.Key, x => x.Value);

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
                this ObservableDictionary<TKey, TValue> source) where TKey : notnull
            => new(source);

        public static ObservableDictionary<TKey, TValue> ToObservableDictionary<TKey, TValue>(
                this Dictionary<TKey, TValue> source) where TKey : notnull
            => [.. source];

        public static ObservableDictionary<TKey, TValue> ToObservableDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector) where TKey : notnull
        {
            var dict = source.ToDictionary(keySelector, valueSelector);
            return [.. dict];
        }
    }
}