using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.Config
{
    public class ConfigRequest : IRequest<ConfigResponse>
    {
        public string AppId { get; set; }
        public string Version { get; set; }
        public string EnvId { get; set; }
        public int? Type { get; set; }
        public string ConfigName { get; set; }
    }
}
