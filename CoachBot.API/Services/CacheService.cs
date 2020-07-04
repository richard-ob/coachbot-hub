using System;
using System.Collections.Generic;

namespace CoachBot.Services
{  
    public class CacheService
    {
        public enum CacheItemType
        {
            LastMention,
            LastUserStatusChangeCheck,
            DiscordVerificationSessionExpiry,
            DiscordVerificationSessionToken
        }

        private Dictionary<CacheItemType, Dictionary<string, object>> _cacheStore = new Dictionary<CacheItemType, Dictionary<string, object>>();

        public CacheService()
        {
            foreach (var itemType in Enum.GetValues(typeof(CacheItemType)))
            {
                _cacheStore.Add((CacheItemType)itemType, new Dictionary<string, object>());
            }
        }

        public object Get(CacheItemType cacheItemType, string key)
        {
            var valueStore = _cacheStore[cacheItemType];
            if (valueStore.TryGetValue(key, out object value))
            {
                return value;
            }

            return null;
        }

        public void Set(CacheItemType cacheItemType, string key, object value)
        {
            var valueStore = _cacheStore[cacheItemType];
            if (valueStore.TryGetValue(key, out object valueStoreValue))
            {
                valueStore[key] = value;
            }
            else
            {
                valueStore.Add(key, value);
            }
        }

        public void Remove(CacheItemType cacheItemType, string key)
        {
            var valueStore = _cacheStore[cacheItemType];
            if (valueStore.TryGetValue(key, out object valueStoreValue))
            {
                valueStore.Remove(key);
            }
        }
    }
}
