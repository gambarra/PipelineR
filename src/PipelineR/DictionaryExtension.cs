using System.Collections.Generic;

namespace PipelineR
{
    public static class ParameterDictionaryExtension
    {
        public static void Add(this Dictionary<string, object> dictionary, string key, object value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        public static T Get<T>(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
                return (T)dictionary[key];
            else
                return default(T);
        }
    }
}