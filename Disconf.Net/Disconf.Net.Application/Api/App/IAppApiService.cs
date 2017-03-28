using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.App
{
    public interface IAppApiService
    {
        Task<ResponseWrapper<AppResponse>> GetAppIdByName(AppRequest request);
    }
}
