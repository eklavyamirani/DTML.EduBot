using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CacheProvider
{
    public interface ICacheProvider
    {
        T Getitem<T>(string key);
        void Additem<T>(string key, T item);

        void Additem<T>(string key, T item, int TimeToLiveSeconds);

        void RemoveItem(string key);

        void ClearCache();

        IEnumerable<T> GetAll<T>();

        IEnumerable<string> GetAllKeys();
    }
}