using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application
{
    public class ResponseWrapper<T>
    {
        public bool HasError => Error != null;

        public ErrorResponse Error { get; set; }

        public T Result { get; set; }

        public readonly string Ok = "OK";

    }
}
