using Disconf.Net.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.App
{
    public class AppApiService : IAppApiService
    {
        private readonly IAppRepository _appRepository;
        public AppApiService(IAppRepository appRepository)
        {
            this._appRepository = appRepository;
        }
        public async Task<ResponseWrapper<AppResponse>> GetAppIdByName(AppRequest request)
        {

            var response = new ResponseWrapper<AppResponse>();
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                response.Error = new ErrorResponse();
            }
            else
            {
                response.Result = new AppResponse
                {
                    AppId = await _appRepository.GetAppIdByName(request.Name)
                };
            }
            return response;
        }
    }
}
