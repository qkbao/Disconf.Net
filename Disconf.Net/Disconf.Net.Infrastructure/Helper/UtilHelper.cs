using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Disconf.Net.Infrastructure.Helper
{
    public static class UtilHelper
    {
        public static string GetRandom()
        {
            Random rad = new Random();
            int value = rad.Next(1000, 10000);
            return value.ToString();
        }

        public static string GetRandomPassword()
        {
            Random rad = new Random();
            var uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var lowers = "abcdefghijklmnopqrstuvwxyz";
            return uppers[rad.Next(0, uppers.Length)].ToString() + lowers[rad.Next(0, lowers.Length)].ToString() + rad.Next(1000, 10000);
        }

        public static string Md5(string password)
        {
            return string.Join("", new MD5CryptoServiceProvider().ComputeHash(new MemoryStream(Encoding.UTF8.GetBytes(password))).Select(x => x.ToString("X2")));
        }

        public static string MD5Of(string text)
        {
            return MD5Of(text, Encoding.Default);
        }

        public static string MD5Of(string text, Encoding enc)
        {
            return HashOf<MD5CryptoServiceProvider>(text, enc);
        }

        public static string SHA1Of(string text)
        {
            return SHA1Of(text, Encoding.Default);
        }

        public static string SHA1Of(string text, Encoding enc)
        {
            return HashOf<SHA1CryptoServiceProvider>(text, enc);
        }

        public static string SHA384Of(string text)
        {
            return SHA384Of(text, Encoding.Default);
        }

        public static string SHA384Of(string text, Encoding enc)
        {
            return HashOf<SHA384CryptoServiceProvider>(text, enc);
        }

        public static string SHA512Of(string text)
        {
            return SHA512Of(text, Encoding.Default);
        }

        public static string SHA512Of(string text, Encoding enc)
        {
            return HashOf<SHA512CryptoServiceProvider>(text, enc);
        }

        public static string SHA256Of(string text)
        {
            return SHA256Of(text, Encoding.Default);
        }

        public static string SHA256Of(string text, Encoding enc)
        {
            return HashOf<SHA256CryptoServiceProvider>(text, enc);
        }

        public static string HashOf<TP>(string text, Encoding enc)
            where TP : HashAlgorithm, new()
        {
            var buffer = enc.GetBytes(text);
            var provider = new TP();
            return BitConverter.ToString(provider.ComputeHash(buffer)).Replace("-", "");
        }

        public static string HttpPost(string url, string postDataStr)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(postDataStr);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string retString = sr.ReadToEnd();

            return retString;
        }

        public static string HttpGet(string url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        //序列化
        //接收4个参数:srcObject(对象的实例),type(对象类型),xmlFilePath(序列化之后的xml文件的绝对路径),xmlRootName(xml文件中根节点名称)
        //当需要将多个对象实例序列化到同一个XML文件中的时候,xmlRootName就是所有对象共同的根节点名称,如果不指定,.net会默认给一个名称(ArrayOf+实体类名称)
        public static void SerializeToXml(object srcObject, Type type, string xmlFilePath, string xmlRootName)
        {
            if (srcObject != null && !string.IsNullOrEmpty(xmlFilePath))
            {
                type = type != null ? type : srcObject.GetType();

                using (StreamWriter sw = new StreamWriter(xmlFilePath))
                {
                    XmlSerializer xs = string.IsNullOrEmpty(xmlRootName) ?
                        new XmlSerializer(type) :
                        new XmlSerializer(type, new XmlRootAttribute(xmlRootName));
                    xs.Serialize(sw, srcObject);
                }
            }
        }

        //反序列化
        //接收2个参数:xmlFilePath(需要反序列化的XML文件的绝对路径),type(反序列化XML为哪种对象类型)
        public static object DeserializeFromXml(string xmlFilePath, Type type)
        {
            object result = null;
            if (File.Exists(xmlFilePath))
            {
                using (StreamReader reader = new StreamReader(xmlFilePath))
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    result = xs.Deserialize(reader);
                }
            }
            return result;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="sPath"></param>
        public static void CreateDirectory(string sPath)
        {
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
        }

        /// <summary>
        /// 随机文件名前缀
        /// </summary>
        /// <returns></returns>
        public static string CreateFileNamePrefix()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }

        /// <summary>
        /// 是否含有被禁止访问的文字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDanger(this string str)
        {
            string word = @"exec|insert|select|delete|update|master|truncate|char|declare|join|iframe|href|script|<|>|request|%|=|--";
            if (str == null)
                return false;
            if (Regex.IsMatch(str, word))
                return true;
            return false;
        }

        /// <summary>
        /// 搜索开始时间格式化
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime FormatSearchStartTime(this DateTime dt)
        {
            return Convert.ToDateTime(dt.ToString("yyyy-MM-dd 00:00:00")).AddDays(-30);
        }

        /// <summary>
        /// 搜索结束时间格式化
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime FormatSearchEndTime(this DateTime dt)
        {
            return Convert.ToDateTime(dt.ToString("yyyy-MM-dd 00:00:00"));
        }

        /// <summary>
        /// 根据格式获取到时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this string date)
        {
            DateTime dt;
            IFormatProvider ifp = new CultureInfo("zh-CN", true);
            DateTime.TryParseExact(date, "yyyyMMdd HHmmss", ifp, DateTimeStyles.None, out dt);
            return dt;
        }

        /// <summary>
        /// 通过格式从字符串获取时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static bool TryParseExact(string time, string format)
        {
            DateTime dt;
            IFormatProvider ifp = new CultureInfo("zh-CN", true);
            var validateResult = DateTime.TryParseExact(time, format, ifp, DateTimeStyles.None, out dt);
            return validateResult;
        }

        /// <summary>
        /// 格式化单元格的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormatCellValue(string str)
        {
            if (str.StartsWith("`"))
            {
                str = str.Remove(0, 1);
            }
            return str;
        }

        /// <summary>
        /// 移除空字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpace(this string str)
        {
            if (str != null)
                return str.Trim();
            return string.Empty;
        }

        /// <summary>
        /// 获取长度的一半
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int Half(this string str)
        {
            return str.Length / 2;
        }
    }
}
