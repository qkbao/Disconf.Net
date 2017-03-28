using Disconf.Net.Client.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Disconf.Net.Client
{
    /// <summary>
    /// Disconf.Net的客户端配置参数
    /// </summary>
    public class ClientConfigSection : ConfigurationSection
    {
        /// <summary>
        /// Rest服务器域名地址
        /// </summary>
        [ConfigurationProperty("webApiHost", IsRequired = true)]
        public string WebApiHost
        {
            get { return this["webApiHost"].ToString(); }
            set { this["webApiHost"] = value; }
        }
        /// <summary>
        /// 是否启用远程配置，默认true，设为false的话表示不从远程服务器下载配置
        /// </summary>
        [ConfigurationProperty("enableRemote", DefaultValue = true)]
        public bool EnableRemote
        {
            get { return Convert.ToBoolean(this["enableRemote"]); }
            set { this["enableRemote"] = value; }
        }

        [ConfigurationProperty("clientInfo", IsRequired = true)]
        public ClientInfoSection ClientInfo
        {
            get { return (ClientInfoSection)this["clientInfo"]; }
        }
        [ConfigurationProperty("updateStrategy")]
        public UpdateStrategySection UpdateStrategy
        {
            get { return (UpdateStrategySection)this["updateStrategy"]; }
        }
        [ConfigurationProperty("directory")]
        public DirectorySection Directory
        {
            get { return (DirectorySection)this["directory"]; }
        }
        #region ConfigurationElement
        /// <summary>
        /// 客户端程序信息
        /// </summary>
        public class ClientInfoSection : ConfigurationElement
        {
            /// <summary>
            /// 客户端程序名称
            /// </summary>
            [ConfigurationProperty("appName", IsRequired = true)]
            public string AppName
            {
                get { return this["appName"].ToString(); }
                set { this["appName"] = value; }
            }
            /// <summary>
            /// 当前客户端程序所处环境
            /// </summary>
            [ConfigurationProperty("environment", IsRequired = true)]
            public string Environment
            {
                get { return this["environment"].ToString(); }
                set { this["environment"] = value; }
            }
            /// <summary>
            /// 当前客户端程序版本
            /// </summary>
            [ConfigurationProperty("version", IsRequired = true)]
            public string Version
            {
                get { return this["version"].ToString(); }
                set { this["version"] = value; }
            }
        }
        /// <summary>
        /// 更新策略
        /// </summary>
        public class UpdateStrategySection : ConfigurationElement
        {
            /// <summary>
            /// 要忽略更新的配置点，以，分割
            /// </summary>
            [ConfigurationProperty("ignores")]
            public string Ignores
            {
                get { return this["ignores"].ToString(); }
                set { this["ignores"] = value; }
            }
            /// <summary>
            /// 第一次是否同步加载，默认同步
            /// </summary>
            [ConfigurationProperty("firstSync", DefaultValue = true)]
            public bool FirstSync
            {
                get { return Convert.ToBoolean(this["firstSync"]); }
                set { this["firstSync"] = value; }
            }
            /// <summary>
            /// 当获取失败时的重试次数，默认为3
            /// </summary>
            [ConfigurationProperty("retryTimes", DefaultValue = 3)]
            public int RetryTimes
            {
                get { return Convert.ToInt32(this["retryTimes"]); }
                set { this["retryTimes"] = value; }
            }
            /// <summary>
            /// 每次重试时间间隔，单位秒，默认每10秒重试一次
            /// </summary>
            [ConfigurationProperty("retryIntervalSeconds", DefaultValue = 10)]
            public int RetryIntervalSeconds
            {
                get { return Convert.ToInt32(this["retryIntervalSeconds"]); }
                set { this["retryIntervalSeconds"] = value; }
            }
        }
        /// <summary>
        /// 文件夹相关
        /// </summary>
        public class DirectorySection : ConfigurationElement
        {
            /// <summary>
            /// 是否绝对路径，默认false
            /// 当false时，表示默认以AppDomain.CurrentDomain.BaseDirectory为比较点
            /// 注意：该配置同时适用于TmpRootDirectory、FactRootDirectory，即要么都只能绝对路径，要么都只能相对路径
            /// </summary>
            [ConfigurationProperty("absolutePath", DefaultValue = false)]
            public bool AbsolutePath
            {
                get { return Convert.ToBoolean(this["absolutePath"]); }
                set { this["absolutePath"] = value; }
            }
            /// <summary>
            /// 下载下来的配置临时保存文件夹根目录，默认为/Tmp/Download/Configs
            /// </summary>
            [ConfigurationProperty("tmpRootDirectory", DefaultValue = "/Tmp/Download/Configs")]
            public string TmpRootDirectory
            {
                get { return this["tmpRootDirectory"].ToString(); }
                set { this["tmpRootDirectory"] = value; }
            }
            /// <summary>
            /// 配置文件实际所在的根目录，默认值为Configs
            /// </summary>
            [ConfigurationProperty("factRootDirectory", DefaultValue = "Configs")]
            public string FactRootDirectory
            {
                get { return this["factRootDirectory"].ToString(); }
                set { this["factRootDirectory"] = value; }
            }
            /// <summary>
            /// 在临时目录下用于保存所有键值对的文件名，注意不要与实际配置文件名字冲突
            /// </summary>
            [ConfigurationProperty("tmpItemsFileName", DefaultValue = "~items.json")]
            public string TmpItemsFileName
            {
                get { return this["tmpItemsFileName"].ToString(); }
                set { this["tmpItemsFileName"] = value; }
            }

            public string TmpRootPhysicalPath
            {
                get { return this.GetPhysicalPath(this.TmpRootDirectory); }
            }
            public string FactRootPhysicalPath
            {
                get { return this.GetPhysicalPath(this.FactRootDirectory); }
            }
            private string GetPhysicalPath(string path)
            {
                var physicalPath = path;
                if (!this.AbsolutePath)
                {
                    physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }
                return physicalPath;
            }

            protected override void PostDeserialize()
            {
                //节点读取成功后，初始化创建相应的文件夹，同时承担路径设置不对的校验
                if (string.Equals(this.TmpRootDirectory, this.FactRootDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("tmpRootDirectory and factRootDirectory can't be same.");
                }
                var tmpRoot = this.TmpRootPhysicalPath;
                var factRoot = this.FactRootPhysicalPath;
                DirectoryHelper.CreateDirectories(tmpRoot, factRoot);
                base.PostDeserialize();
            }
        }
        #endregion

        protected override void PostDeserialize()
        {
            Uri uri = new Uri(this.WebApiHost);//用来验证uri是否正确
            base.PostDeserialize();
        }

        public static ClientConfigSection Current
        {
            get { return (ClientConfigSection)ConfigurationManager.GetSection("clientConfig"); }
        }
    }
}
