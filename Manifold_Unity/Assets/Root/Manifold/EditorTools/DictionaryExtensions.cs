using System;
using System.Collections.Generic;

namespace Manifold.EditorTools
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> AddArraysAsKeyValuePairs<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey[] keys, TValue[] values)
        {
            if (dictionary is null)
                throw new ArgumentNullException(nameof(dictionary));

            bool areSameLengths = keys.Length == values.Length;
            if (!areSameLengths)
                throw new ArgumentException("Length of keys and values are not the same.");

            for (int i = 0; i < keys.Length; i++)
                dictionary.Add(keys[i], values[i]);

            return dictionary;
        }

    }
}
