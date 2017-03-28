using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Client.Rules
{
    public interface IRule
    {
        /// <summary>
        /// 注册Rule规则，设置默认的属性映射
        /// </summary>
        /// <param name="configName">默认采用远程的configName</param>
        /// <returns></returns>
        IRule MapTo(string configName);
        /// <summary>
        /// 当远程配置的值发生变化时，通知值变更
        /// </summary>
        /// <param name="changedValue"></param>
        void ConfigChanged(string changedValue);
    }
}
