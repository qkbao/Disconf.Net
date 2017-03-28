using Disconf.Net.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.ConsoleTest
{
    class DisConfigRules
    {
        public static void Register()
        {
            Register(ConfigManager.Instance);
        }
        public static void Register(ConfigManager manager)
        {
            //要更新的文件
            //manager.FileRules.For("appSettings.config").CallBack(() =>
            manager.FileRules.For("appSettings.config").CallBack(() =>
            {
                Console.WriteLine("File changed at:{0:yy-MM-dd HH:mm:ss fff}", DateTime.Now);
                Console.WriteLine("Now setting data is `aaa`:{0}  `bbb`:{1}", ConfigurationManager.AppSettings["aaa"], ConfigurationManager.AppSettings["bbb"]);
            }).CallBack(() => {
                //委托链的方式
                Console.WriteLine("File changed  notice twice");
            });
            //要更新的键值对
            manager.ItemRules.For("PropMap").MapTo("Person").SetStaticProperty<Program>().CallBack(v =>
            {
                Console.WriteLine("Now item value:{0}", v);
                Console.WriteLine("Program.Person is {0} now", Program.Person);
                //if (v.Length > 3)
                //{
                //    throw new Exception("Too Long");
                //}
            });

            //忽略更新到本地的键值对
            manager.ItemRules.For("Peng").CallBack(v =>
            {
                Console.WriteLine("Peng 's value is:{0}", v);
            });

            manager.Faulted += Manager_Faulted;

            manager.Init();
        }

        private static void Manager_Faulted(Exception arg2)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(arg2);
            Console.ForegroundColor = color;
        }
    }
}
