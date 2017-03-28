using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;

namespace Disconf.Net.Infrastructure.Data
{
    public static partial class Database
    {
        private static object priLock = new object();

        // Change this key for each project. We're using Triple DES for encrypting connectionstrings, but you easily
        // change the CryptoServiceProvider in Sources.EncryptConnectionString(). For Single DES the key needs fixed
        // size of 8 bytes. For Triple DES it is 24 (192bit). Rijndael supports 128, 192 and 256 bit keysizes.
        private const string encryptionkey = "02642974-49f4-B5F2-46AE-C0C98CF6EFCB"; // Just a GUID >= 24 characters.

        #region Open Connection

        /// <summary>
        /// Returns an open connection by its name from app.config, web.config, settings.settings or imported xml-store.
        /// </summary>
        /// <param name="connectionName">Name of connectionString setting in configuration store.</param>
        /// <returns>Returns a new instance of the provider's class that represents a connection to the database.</returns>
        /// <remarks>Please wrap in a using statement. It's best practice to close and dispose of database connections
        /// as soon as you're done with it. Connection pooling will kick in and take care of live connections for you.</remarks>
        public static DbConnection Open(string connectionName)
        {
            DbConnection connection = Connection(connectionName);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Imports an xml store (e.g. databases.xml) with connection settings and returns the requested connection opened.
        /// </summary>
        /// <param name="configurationFileName">Full or relative file path. (e.g. C:\databases.xml)</param>
        /// <param name="connectionName">Returns a new instance of the provider's class that represents a connection to the database.</param>
        /// <returns>Returns a new instance of the provider's class that represents a connection to the database.</returns>
        public static DbConnection Open(string configurationFileName, string connectionName)
        {
            DbConnection connection = Connection(configurationFileName, connectionName);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Imports an xml store by it's relative path in ASP.NET and returns the requested connection.
        /// Does not open the connection.
        /// </summary>
        /// <param name="httpContext">Context of HTTP request to resolve relative path with. (e.g. HttpContext.Current)</param>
        /// <param name="configurationFileName">Full or relative server path. (e.g. ~/App_Data/databases.xml)</param>
        /// <param name="connectionName">Returns a new instance of the provider's class that represents a connection to the database.</param>
        public static DbConnection Open(HttpContextBase httpContext, string configurationFileName, string connectionName)
        {
            DbConnection connection = Connection(httpContext, configurationFileName, connectionName);
            connection.Open();
            return connection;
        }

        #endregion Open Connection

        #region Closed Connection

        /// <summary>
        /// Returns connection by its name from app.config, web.config, settings.settings or imported xml-store.
        /// Does not open the connection.
        /// </summary>
        /// <param name="connectionName">Name of connectionString setting in configuration store.</param>
        /// <returns>Returns a new instance of the provider's class that represents a connection to the database.</returns>
        /// <remarks>Please wrap in a using statement. It's best practice to close and dispose of database connections
        /// as soon as you're done with it. Connection pooling will kick in and take care of live connections for you.</remarks>
        public static DbConnection Connection(string connectionName)
        {
            Sources.Configuration configuration = Sources.Find(connectionName);
            DbConnection connection = configuration.Provider.CreateConnection();
            connection.ConnectionString = configuration.ConnectionString;
            return connection;
        }

        /// <summary>
        /// Imports an xml store (e.g. databases.xml) with connection settings and returns the requested connection.
        /// Does not open the connection.
        /// </summary>
        /// <param name="configurationFileName">Full or relative file path. (e.g. C:\databases.xml)</param>
        /// <param name="connectionName">Returns a new instance of the provider's class that represents a connection to the database.</param>
        /// <returns>Returns a new instance of the provider's class that represents a connection to the database.</returns>
        public static DbConnection Connection(string configurationFileName, string connectionName)
        {
            Sources.ImportXmlStore(configurationFileName);
            return Connection(connectionName);
        }

        /// <summary>
        /// Imports an xml store by it's relative path in ASP.NET and returns the requested connection.
        /// Does not open the connection.
        /// </summary>
        /// <param name="httpContext">Context of HTTP request to resolve relative path with. (e.g. HttpContext.Current)</param>
        /// <param name="configurationFileName">Full or relative server path. (e.g. ~/App_Data/databases.xml)</param>
        /// <param name="connectionName">Returns a new instance of the provider's class that represents a connection to the database.</param>
        /// <returns>Returns a new instance of the provider's class that represents a connection to the database.</returns>
        public static DbConnection Connection(HttpContextBase httpContext, string configurationFileName, string connectionName)
        {
            Sources.ImportXmlStore(httpContext.Server.MapPath(configurationFileName));
            return Connection(connectionName);
        }

        public static Sources.Configuration GetConfiguration(string connectionName)
        {
            return Sources.Find(connectionName);
        }

        #endregion Closed Connection

        /// <summary>
        /// Keeps a collection of database sources settings from configuration files typically used in .NET applications.
        /// Additionally instantiates required data source provider classes and keeps those around until the app domain
        /// is unloaded or Database.Sources.Clear() is called and all dependent connections are closed and disposed of.
        /// </summary>
        public static class Sources
        {
            private static Lazy<List<Configuration>> _configurations = new Lazy<List<Configuration>>();

            private static Lazy<Dictionary<string, DbProviderFactory>> _providers = new Lazy<Dictionary<string, DbProviderFactory>>();

            private static Lazy<List<string>> _xmlStores = new Lazy<List<string>>();

            /// <summary>
            /// Databases is a static helper class that keeps a reference to instantiated database providers once used. This means it
            /// keeps a couple of objects alive in memory while the assembly is loaded. That is potentially a behaviour you do not need
            /// or like. You can however flush all settings and references to instantiated providers by calling Clear() when done.
            /// Required data source providers will build again when you create a connection. But most projects are likely to create
            /// connections with the same provider classes continuesly (for new queries), which is especially true for web applications.
            /// </summary>
            public static void Clear()
            {
                if (_configurations.IsValueCreated) _configurations.Value.Clear();
                if (_xmlStores.IsValueCreated) _xmlStores.Value.Clear();
                if (_providers.IsValueCreated) _providers.Value.Clear();
            }

            /// <summary>
            /// Decrypts a ciphered connection string. This is done for you while reading the configs.
            /// </summary>
            /// <param name="cipheredConnectionString">encrypted:0jx0NNG6POnEZ4/5VKXfeUj0u5WhEa9AEdPx7mYrIiFGmPNPJw8dVZvrcc8gjuy35mz/lt8M2s4e9dQFXHZzgQ##</param>
            /// <returns>Server=localhost;User=myusername;Password=mypassword;Charser=NONE;Database=C:\Database.fdb</returns>
            public static string DecryptConnectionString(string cipheredConnectionString)
            {
                cipheredConnectionString = cipheredConnectionString.Replace("encrypted:", String.Empty);

                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                byte[] tdesKey = encoding.GetBytes(encryptionkey.Substring(0, 24));
                byte[] passwordBytes = Convert.FromBase64String(cipheredConnectionString);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = tdesKey;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateDecryptor();

                byte[] passwordArray = cTransform.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
                tdes.Clear();
                return System.Text.UTF8Encoding.UTF8.GetString(passwordArray);
            }

            /// <summary>
            /// Encrypts a connection string to use in datasource configs. See example in Databases.cs.
            /// </summary>
            /// <param name="connectionString">Server=localhost;User=myusername;Password=mypassword;Charser=NONE;Database=C:\Database.fdb</param>
            /// <returns>encrypted:0jx0NNG6POnEZ4/5VKXfeUj0u5WhEa9AEdPx7mYrIiFGmPNPJw8dVZvrcc8gjuy35mz/lt8M2s4e9dQFXHZzgQ##</returns>
            public static string EncryptConnectionString(string connectionString)
            {
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                byte[] tdesKey = encoding.GetBytes(encryptionkey.Substring(0, 24));
                byte[] passwordBytes = encoding.GetBytes(connectionString);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = tdesKey;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateEncryptor();

                byte[] passwordArray = cTransform.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
                tdes.Clear();
                return Convert.ToBase64String(passwordArray, 0, passwordArray.Length);
            }

            /// <summary>
            /// Finds a single database source configuration by its name parameter in app.config, web.config, settings.settings
            /// and imported databases.xml stores.
            /// </summary>
            /// <returns>Returns a database configuration with instantiated data source provider.</returns>
            /// <exception cref="System.Configuration.ConfigurationErrorsException">Thrown when configuration not found.</exception>
            internal static Configuration Find(string name)
            {
                // Search configurations used and imported from xml stores.
                lock (priLock)
                {
                    Configuration configuration = _configurations.Value.Find(c => c.Name == name);
                    if (configuration != null) return configuration;
                    // TODO: Search configurations in settings.setting.

                    // Search configurations in web.config and <applicationname>.config.
                    var newconfiguration = new Configuration()
                    {
                        Name = name,
                        ConnectionString = WebConfigurationManager.ConnectionStrings[name].ConnectionString,
                        ProviderName = WebConfigurationManager.ConnectionStrings[name].ProviderName,
                        Provider = AddProvider(WebConfigurationManager.ConnectionStrings[name].ProviderName)
                    };

                    _configurations.Value.Add(newconfiguration);
                    return newconfiguration;
                }
            }

            /// <summary>
            /// Takes connection settings from a specified XML store (e.g. databases.xml) and includes those as database sources.
            /// Connection settings from web.config and app.config are retrieved by default with WebConfigurationManager. Stores
            /// are added once only, so there's no harm in calling this often. However, there is a some overhead in checking.
            /// </summary>
            /// <param name="configurationFileName">Path to XML store with database settings (e.g. "C:\\databases.xml").</param>
            internal static void ImportXmlStore(string configurationFileName)
            {
                if (!_xmlStores.Value.Contains(configurationFileName, StringComparer.CurrentCultureIgnoreCase))
                {
                    var doc = XDocument.Load(configurationFileName);
                    _configurations.Value.AddRange(doc.Root == null
                        ? Enumerable.Empty<Configuration>()
                        : (from databaseElement in doc.Root.Elements("database")
                           select new Configuration
                           {
                               Name = (string)databaseElement.Element("name"),
                               ConnectionString = (string)databaseElement.Element("connectionString"),
                               ProviderName = (string)databaseElement.Element("providerName"),
                               Provider = AddProvider((string)databaseElement.Element("providerName"))
                           })
                    );
                    _xmlStores.Value.Add(configurationFileName);
                }
            }

            /// <summary>
            /// Adds data source provider to a static dictionary to reuse the same factory for multiple connections.
            /// Returns an existing provider of the same class (e.g. MySql.Data.MySqlClient) if already instantiated.
            /// </summary>
            private static DbProviderFactory AddProvider(string providerName)
            {
                if (_providers.Value.ContainsKey(providerName))
                {
                    return _providers.Value[providerName];
                }
                else
                {
                    DbProviderFactory provider;
                    _providers.Value.Add(providerName, provider = DbProviderFactories.GetFactory(providerName));
                    return provider;
                }
            }

            public class Configuration
            {
                private string _connectionString = String.Empty;

                public string ConnectionString
                {
                    get
                    {
                        return _connectionString;
                    }

                    set
                    {
                        _connectionString = value.StartsWith("encrypted:") ? Sources.DecryptConnectionString(value) : value;
                    }
                }

                // Settings from configuration files.
                public string Name { get; set; }

                // Reference to provider classes, instance is shared.
                public DbProviderFactory Provider { get; set; }

                public string ProviderName { get; set; }
            }
        }
    }
}
