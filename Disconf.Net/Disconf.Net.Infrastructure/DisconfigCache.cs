using Disconf.Net.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Infrastructure
{
    public class DisconfigCache
    {
        private static CacheItemPolicy policy = new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddSeconds(AppSettingHelper.Get<int>("CacheExpire")) };
        private static ObjectCache _cache = MemoryCache.Default;
        public static bool Add<T>(string key, T value)
        {
            return _cache.Add(key, value, policy);
        }

        public static void Set<T>(string key, T value)
        {
            _cache.Set(key, value, policy);
        }

        public static object Get(string key)
        {
            return _cache.Get(key);
        }

        public static void Delete(string key)
        {
            _cache.Remove(key);
        }

        public static void UpdateCache(string key, string value, int type, string name)
        {
            var cache = Get(key);
            if (cache != null)
            {
                var dic = (Dictionary<int, Dictionary<string, string>>)cache;
                if (dic.ContainsKey(type))
                    if (dic[type].ContainsKey(name))
                        dic[type][name] = value;
            }
        }

        public static void AddCache(string key, string value, int type, string name)
        {
            var cache = Get(key);
            if (cache != null)
            {
                var dic = (Dictionary<int, Dictionary<string, string>>)cache;
                if (dic.ContainsKey(type))
                    dic[type].Add(name, value);
            }
        }

        public static void DeleteCache(string key, int type, string name)
        {
            var cache = Get(key);
            if (cache != null)
            {
                var dic = (Dictionary<int, Dictionary<string, string>>)cache;
                if (dic.ContainsKey(type))
                    dic[type].Remove(name);
            }
        }
    }
}
