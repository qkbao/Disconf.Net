using Disconf.Net.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Disconf.Net.Client.Fetch
{
    /// <summary>
    /// 数据抓取接口，用于定义从Web服务器获取数据和文件
    /// </summary>
    public interface IFetcher
    {
        /// <summary>
        /// 根据键值获取对应的Config内容
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string GetConfig(ConfigFetchFilter filter);
        /// <summary>
        /// 批量获取所有的配置节
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string GetAllConfigs(FetchFilter filter);
        /// <summary>
        /// 获取Zookeeper服务路径
        /// </summary>
        /// <returns></returns>
        string GetZkHosts();
        /// <summary>
        /// 获取指定应用的最后一次更新时间
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string GetLastChangedTime(FetchFilter filter);
    }
}
