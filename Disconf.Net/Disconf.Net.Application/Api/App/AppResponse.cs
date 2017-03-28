using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.App
{
    public class AppResponse : AbstractResponse
    {
        public string AppId { get; set; }
    }
}
