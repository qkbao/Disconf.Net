using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Client
{
    internal class ExceptionHandler
    {
        public event Action<string, Exception> Faulted;
        public void Execute(Action action, string errorMsg = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (this.Faulted != null)
                {
                    try
                    {//防止事件方法内部错误导致程序报错
                        this.Faulted(errorMsg, ex);
                    }
                    catch { }
                }
            }
        }
    }
}
