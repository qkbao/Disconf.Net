using Disconf.Net.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.Zookeeper
{
    public class ZookeeperService : IZookeeperService
    {
        public ResponseWrapper<ZooKeeperResponse> GetZookeeperHost()
        {

            var response = new ResponseWrapper<ZooKeeperResponse>();
            response.Result = new ZooKeeperResponse
            {
                Host = AppSettingHelper.Get<string>("ZookeeperHost")
            };
            return response;
        }
    }
}
