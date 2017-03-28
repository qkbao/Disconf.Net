using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Client.Rules
{
    public class ItemRule : IItemRule
    {
        /// <summary>
        /// 默认属性名
        /// </summary>
        internal string DefaultPropName { get; private set; }
        /// <summary>
        /// 远程配置值变更时要执行的委托
        /// </summary>
        internal Action<string> Action { get; private set; }
        private List<PropertyMap> _list = new List<PropertyMap>();
        
        public IItemRule CallBack(Action<string> action)
        {
            if (action != null)
            {
                if (this.Action == null)
                {
                    this.Action = action;
                }
                else
                {
                    this.Action += action;
                }
            }
            return this;
        }

        public void ConfigChanged(string changedValue)
        {
            if (this._list != null && this._list.Count > 0)
            {
                foreach (var map in this._list)
                {
                    try
                    {
                        object value;
                        if (map.TypeConvert != null)
                        {
                            value = map.TypeConvert(changedValue);
                        }
                        else
                        {
                            value = Convert.ChangeType(changedValue, map.PropertyInfo.PropertyType);
                        }
                        map.PropertyInfo.SetValue(map.Entity, value);
                    }
                    catch { }
                }
            }
            if (this.Action != null)
            {
                this.Action(changedValue);
            }
        }

        public IItemRule MapTo(string propName)
        {
            if (!string.IsNullOrWhiteSpace(propName))
            {
                this.DefaultPropName = this.GetPropName(propName);
            }
            return this;
        }

        private string GetPropName(string propName)
        {
            int idx = propName.LastIndexOf('.');
            if (idx >= 0)
            {
                return propName.Substring(idx + 1);
            }
            return propName;
        }

        public IItemRule SetProperty(object entity, PropertyInfo prop, Func<string, object> typeConvert = null)
        {
            this._list.Add(new PropertyMap
            {
                Entity = entity,
                PropertyInfo = prop,
                TypeConvert = typeConvert
            });
            return this;
        }

        public IItemRule SetProperty<T>(T entity, string propName = null, Func<string, object> typeConvert = null)
        {
            PropertyInfo prop = typeof(T).GetProperty(string.IsNullOrWhiteSpace(propName) ? this.DefaultPropName : propName);
            if (prop != null)
            {
                return this.SetProperty(entity, prop, typeConvert);
            }
            return this;
        }

        public IItemRule SetStaticProperty(PropertyInfo prop, Func<string, object> typeConvert = null)
        {
            return this.SetProperty(null, prop, typeConvert);
        }

        public IItemRule SetStaticProperty<T>(string propName = null, Func<string, object> typeConvert = null)
        {
            return this.SetProperty<T>(default(T), propName, typeConvert);
        }
        IRule IRule.MapTo(string configName)
        {
            return this.MapTo(configName);
        }

        private class PropertyMap
        {
            public object Entity { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
            public Func<string, object> TypeConvert { get; set; }
        }
    }
}
