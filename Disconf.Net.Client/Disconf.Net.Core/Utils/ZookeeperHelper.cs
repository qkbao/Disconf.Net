using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace Disconf.Net.Core.Utils
{
    public static class ZookeeperHelper
    {
        public static string CreateWithPath(this ZooKeeper zk, string path, byte[] data, IEnumerable<ACL> acl, CreateMode createMode)
        {
            var tmp = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (path.Length > 1)
            {
                string tmpPath = "";
                for (var i = 0; i < tmp.Length - 1; i++)
                {
                    tmpPath += "/" + tmp[i];
                    if (zk.Exists(tmpPath, false) == null)
                    {
                        zk.Create(tmpPath, null, acl, createMode);//临时节点目前不允许存在子节点，所以这里可能会有问题
                    }
                }
            }
            return zk.Create(path, data, acl, createMode);
        }
    }
}
