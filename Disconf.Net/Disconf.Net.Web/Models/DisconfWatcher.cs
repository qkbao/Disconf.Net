using Disconf.Net.Core.Model;
using Disconf.Net.Core.Zookeeper;
using Disconf.Net.Infrastructure.Helper;
using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Disconf.Net.Web.Models
{
    public class DisconfWatcher
    {
        private static string host = AppSettingHelper.Get<string>("ZookeeperHost");
        private static MaintainWatcher maintainWatcher = new MaintainWatcher(host, 10000);

        public static void AddOrSetData(string path)
        {
            maintainWatcher.AddOrSetData(path, BitConverter.GetBytes(DateTime.Now.Ticks));
        }

        public static void Remove(string path)
        {
            maintainWatcher.Remove(path);
        }

        public static string GetPath(string configName, string appName, string version, string env, int type)
        {
            ZkTreeBuilder zkBuilder;
            if (type == (int)ConfigType.File)
                zkBuilder = new ZkFileTreeBuilder(appName, version, env);
            else
                zkBuilder = new ZkItemTreeBuilder(appName, version, env);
            var nodeName = zkBuilder.GetOrAddZnodeName(configName);
            return zkBuilder.GetZkPath(nodeName);
        }

        public static List<string> GetChildren(string path)
        {
            var list = new List<string>();
            try
            {
                var childs = maintainWatcher.ZooKeeper.GetChildren(path, false);
                foreach (var item in childs)
                {
                    try
                    {
                        var node = maintainWatcher.ZooKeeper.GetData(string.Format("{0}/{1}", path, item), false, new Stat());
                        if (node.Any())
                        {
                            list.Add(Encoding.Default.GetString(node));
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return list;
        }
    }
}