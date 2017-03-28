using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Client.Rules
{
    public class RuleCollection<T>
        where T : IRule, new()
    {
        private ConcurrentDictionary<string, T> _rules = new ConcurrentDictionary<string, T>();
        /// <summary>
        /// 根据configName获取对应的Rule对象
        /// </summary>
        /// <param name="configName">注意configName区分大小写</param>
        /// <returns></returns>
        public T For(string configName)
        {
            T rule = this._rules.GetOrAdd(configName, _ =>
            {
                var t = new T();
                t.MapTo(_);
                return t;
            });
            return rule;
        }

        public void ConfigChanged(string configName, string changedValue)
        {
            T rule;
            if (this._rules.TryGetValue(configName, out rule))
            {
                rule.ConfigChanged(changedValue);
            }
        }
    }
}
