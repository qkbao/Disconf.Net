using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.Zookeeper
{
    public interface IZookeeperService
    {
        ResponseWrapper<ZooKeeperResponse> GetZookeeperHost();
    }
}
