using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Client.Rules
{
    public interface IFileRule : IRule
    {
        /// <summary>
        /// 当文件下载完成并且替换本地对应文件后回调，注意此处将采用委托链的方式，即多次调用均会被执行
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IFileRule CallBack(Action action);
        /// <summary>
        /// 注册Rule规则，设置默认的文件配置映射
        /// </summary>
        /// <param name="refreshSectionName">更新回调时ConfigurationManager.RefreshSection要刷新的节点名称，默认采用远程配置的configName</param>
        /// <returns></returns>
        new IFileRule MapTo(string refreshSectionName);
        /// <summary>
        /// 不自动调用ConfigurationManager.RefreshSection方法更新配置
        /// </summary>
        /// <returns></returns>
        IFileRule RefreshIgnores();
    }
}
