using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CacheProvider
{
    public static class Cache
    {
        public static ICacheProvider cacheStorage = new MemoryCache();
    }

    public class MemoryCache : ICacheProvider
    {
        private static ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();
        private int CacheLimit = 500;
        private int DefaultTimeToLive = 5 * 60 * 60; // 5 hours

        public T Getitem<T>(string key)
        {
            if (cache.ContainsKey(key))
            {
                var value = ((Tuple<DateTime, T>)cache[key]);
                if (DateTime.Now > value.Item1)
                {
                    this.RemoveItem(key);
                    return default(T);
                }

                return value.Item2;
            }
            else
            {
                return default(T);
            }
        }

        public void Additem<T>(string key, T item, int TimeToLive)
        {
            if (!cache.ContainsKey(key))
            {
                if (!cache.ContainsKey(key))
                {
                    if (cache.Count > CacheLimit) cache.Clear();
                    cache.TryAdd(key, new Tuple<DateTime, T>(DateTime.Now.AddSeconds(TimeToLive), item));
                }
            }
        }

        public void Additem<T>(string key, T item)
        {
            Additem<T>(key, item, DefaultTimeToLive);
        }

        public void RemoveItem(string key)
        {
            object value;
            cache.TryRemove(key, out value);
        }

        public void ClearCache()
        {
            cache.Clear();
        }


        public IEnumerable<T> GetAll<T>()
        {
            return cache.Values.Select(a => ((Tuple<DateTime, T>)a).Item2).ToList();
        }


        public IEnumerable<string> GetAllKeys()
        {
            return cache.Keys.ToList();
        }
    }
}