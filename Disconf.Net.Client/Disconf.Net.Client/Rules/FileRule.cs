using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Client.Rules
{
    public class FileRule : IFileRule
    {
        public FileRule()
        {
            this.AutoRefresh = true;
        }
        /// <summary>
        /// 要刷新的节点名称
        /// </summary>
        internal string SectionName { get; private set; }
        /// <summary>
        /// 是否自动刷新，默认自动刷新
        /// </summary>
        internal bool AutoRefresh { get; private set; }
        /// <summary>
        /// 远程配置值变更时要执行的委托
        /// </summary>
        internal Action Action { get; private set; }
        public IFileRule CallBack(Action action)
        {
            if (action != null)
            {
                this.Action += action;
            }
            return this;
        }

        public void ConfigChanged(string changedValue)
        {
            if (this.AutoRefresh)
            {
                ConfigurationManager.RefreshSection(this.SectionName);
            }
            if (this.Action != null)
            {
                this.Action();
            }
        }

        public IFileRule MapTo(string refreshSectionName)
        {
            if (!string.IsNullOrWhiteSpace(refreshSectionName))
            {
                this.SectionName = Path.GetFileNameWithoutExtension(refreshSectionName);
            }
            return this;
        }

        public IFileRule RefreshIgnores()
        {
            this.AutoRefresh = false;
            return this;
        }

        IRule IRule.MapTo(string configName)
        {
            return this.MapTo(configName);
        }
    }
}
