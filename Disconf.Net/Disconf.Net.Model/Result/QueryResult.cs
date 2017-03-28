using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Model.Result
{
    [Serializable]
    public class QueryResult<T>
    {
        public bool IsSuccess { get; set; }
        public string ErrorMsg { get; set; }
        public T Data { get; set; }
        public int Count { get; set; }
    }
}
