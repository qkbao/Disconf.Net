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
        /// <summary>
        /// 业务系统信息设置
        /// </summary>
        [ConfigurationProperty("clientInfo", IsRequired = true)]
        public ClientInfoSection ClientInfo
        {
            get { return (ClientInfoSection)this["clientInfo"]; }
        }
        /// <summary>
        /// 更新策略设置
        /// </summary>
        [ConfigurationProperty("updateStrategy")]
        public UpdateStrategySection UpdateStrategy
        {
            get { return (UpdateStrategySection)this["updateStrategy"]; }
        }
        /// <summary>
        /// 本地持久化设置
        /// </summary>
        [ConfigurationProperty("preservation")]
        public PreservationSection Preservation
        {
            get { return (PreservationSection)this["preservation"]; }
        }
        #region ConfigurationElement
        /// <summary>
        /// 客户端程序信息
        /// </summary>
        public class ClientInfoSection : ConfigurationElement
        {
            /// <summary>
            /// 客户端程序名称，注意大小写要与服务端一致
            /// </summary>
            [ConfigurationProperty("appName", IsRequired = true)]
            public string AppName
            {
                get { return this["appName"].ToString(); }
                set { this["appName"] = value; }
            }
            /// <summary>
            /// 当前客户端程序所处环境，注意大小写要与服务端一致
            /// </summary>
            [ConfigurationProperty("environment", IsRequired = true)]
            public string Environment
            {
                get { return this["environment"].ToString(); }
                set { this["environment"] = value; }
            }
            /// <summary>
            /// 当前客户端程序版本，注意大小写要与服务端一致
            /// </summary>
            [ConfigurationProperty("version", IsRequired = true)]
            public string Version
            {
                get { return this["version"].ToString(); }
                set { this["version"] = value; }
            }
            /// <summary>
            /// 客户端标识，用于服务端查看已更新客户端，如果不设置则默认获取客户端电脑名称
            /// </summary>
            [ConfigurationProperty("clientName", DefaultValue = null)]
            public string ClientName
            {
                get { return this["clientName"].ToString(); }
                set { this["clientName"] = value; }
            }
        }
        /// <summary>
        /// 更新策略
        /// </summary>
        public class UpdateStrategySection : ConfigurationElement
        {
            /// <summary>
            /// 要忽略更新的文件配置，以,分割，注意大小写要与服务端一致
            /// </summary>
            [ConfigurationProperty("fileIgnores", DefaultValue = "")]
            public string FileIgnores
            {
                get { return this["fileIgnores"].ToString(); }
                set { this["fileIgnores"] = value; }
            }
            /// <summary>
            /// 要忽略更新的键值对配置，以,分割，注意大小写要与服务端一致
            /// </summary>
            [ConfigurationProperty("itemIgnores", DefaultValue = "")]
            public string ItemIgnores
            {
                get { return this["itemIgnores"].ToString(); }
                set { this["itemIgnores"] = value; }
            }
            /// <summary>
            /// 启动时是否同步加载，默认同步
            /// </summary>
            [ConfigurationProperty("startedSync", DefaultValue = true)]
            public bool StartedSync
            {
                get { return Convert.ToBoolean(this["startedSync"]); }
                set { this["startedSync"] = value; }
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
            public string[] FileIgnoreList { get; private set; }
            public string[] ItemIgnoreList { get; private set; }
            protected override void PostDeserialize()
            {
                this.FileIgnoreList = this.FileIgnores.Split(',');
                this.ItemIgnoreList = this.ItemIgnores.Split(',');
                base.PostDeserialize();
            }
        }
        /// <summary>
        /// 本地持久化设置相关
        /// </summary>
        public class PreservationSection : ConfigurationElement
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
            /// 下载下来的配置临时保存文件夹根目录，默认为Tmp/Download/Configs
            /// </summary>
            [ConfigurationProperty("tmpRootDirectory", DefaultValue = "Tmp/Download/Configs")]
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
            /// 在临时目录下用于保存所有键值对的文件名，设置为空表示不保存
            /// 为方便服务器配置发生变更时进行对应值的修改，这里存储格式为xml
            /// 文件保存在TmpRootDirectory目录下，所以注意不要与实际配置文件名字冲突
            /// </summary>
            [ConfigurationProperty("tmpItemsLocalName", DefaultValue = "~items.xml")]
            public string TmpItemsLocalName
            {
                get { return this["tmpItemsLocalName"].ToString(); }
                set { this["tmpItemsLocalName"] = value; }
            }
            /// <summary>
            /// 在临时目录下用于保存所有文件配置名的文件名，设置为空表示不保存
            /// 因为运行中不存在修改的可能性，所以此部分直接简单的存储为文本格式，多个文件名之间以,分隔
            /// 文件保存在TmpRootDirectory目录下，所以注意不要与实际配置文件名字冲突
            /// </summary>
            [ConfigurationProperty("tmpFilesLocalName", DefaultValue = "~files.txt")]
            public string TmpFilesLocalName
            {
                get { return this["tmpFilesLocalName"].ToString(); }
                set { this["tmpFilesLocalName"] = value; }
            }
            /// <summary>
            /// 在临时目录下用于保存上次取到的最后一项配置修改时间，设置为空会导致每次都从服务器拉取全部配置
            /// </summary>
            [ConfigurationProperty("tmpTimeLocalName", DefaultValue = "~time.txt")]
            public string TmpTimeLocalName
            {
                get { return this["tmpTimeLocalName"].ToString(); }
                set { this["tmpTimeLocalName"] = value; }
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
                if (string.IsNullOrWhiteSpace(this.TmpRootDirectory))
                {
                    throw new ArgumentException("tmpRootDirectory can't be empty.");
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
