using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Disconf.Net.Web.Filters;
using System.IO;
using System.Text;
using Ionic.Zip;
using Disconf.Net.Infrastructure.Helper;

namespace Disconf.Net.Web.Controllers
{
    [ActionFilter]
    public class BaseController : Controller
    {
        public static readonly int PageSize = 10;

        public long AppId
        {
            get
            {
                return long.Parse(HttpContext.Session["AppId"].ToString());
            }
        }
        public long EnvId
        {
            get
            {
                return long.Parse(HttpContext.Session["EnvId"].ToString());
            }
        }

        /// <summary>
        /// 压缩zip
        /// </summary>
        /// <param name="fileToZips">文件路径集合</param>
        /// <param name="zipedFile">想要压成zip的文件名</param>
        public void Zip(string[] fileToZips, string zipedFile)
        {
            using (ZipFile zip = new ZipFile(AppSettingHelper.Get<string>("ZipPath") + zipedFile + ".zip", Encoding.Default))
            {
                foreach (string fileToZip in fileToZips)
                {
                    using (FileStream fs = new FileStream(fileToZip, FileMode.Open, FileAccess.ReadWrite))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                        zip.AddEntry(fileName, buffer);
                    }
                }
                zip.Save();
            }
        }
    }
}