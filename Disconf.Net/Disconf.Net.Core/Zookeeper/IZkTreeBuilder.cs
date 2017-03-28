using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Core.Zookeeper
{
    public interface IZkTreeBuilder
    {
        /// <summary>
        /// 根据配置名称获取对应的Zookeeper下的节点名称，如果无法找到对应节点则添加并返回对应节点名称，该方法用于解决路径问题
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        string GetOrAddZnodeName(string configName);
        /// <summary>
        /// 根据Zookeeper的节点名称获取对应的配置名称，该方法用于watch到变更时，如何反向处理获取配置，如果无法找到对应的znode，则返回null
        /// </summary>
        /// <param name="znodeName">zk的节点名称</param>
        /// <returns></returns>
        string GetConfigName(string znodeName);
        /// <summary>
        /// 获取指定应用在zk中的根路径
        /// </summary>
        /// <returns></returns>
        string GetZkRootPath();
        /// <summary>
        /// 获取指定节点名称在zookeeper中的完整路径，注意是znodeName，而不是configName
        /// </summary>
        /// <param name="znodeName"></param>
        /// <returns></returns>
        string GetZkPath(string znodeName);
        /// <summary>
        /// 获取所有已配置的znodeName
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetAllZnodes();
    }
}
