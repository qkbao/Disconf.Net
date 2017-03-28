using System;
using System.ComponentModel;
using System.Configuration;

namespace Disconf.Net.Infrastructure.Helper
{
    public class AppSettingHelper
    {
        public static T Get<T>(string key)
        {
            var appSetting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting)) throw new Exception(String.Format("Could not find setting '{0}',", key)); ;

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)(converter.ConvertFromInvariantString(appSetting));
        }
    }
}
