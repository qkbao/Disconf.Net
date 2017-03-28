using DapperExtensions;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Core.Model;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using Disconf.Net.Infrastructure;
using Disconf.Net.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.Config
{
    public class ConfigApiService : IConfigApiService
    {
        private readonly IConfigRepository _configRepository;
        private readonly IAppRepository _appRepository;
        public ConfigApiService(IConfigRepository configRepository, IAppRepository appRepository)
        {
            this._configRepository = configRepository;
            this._appRepository = appRepository;
        }
        public async Task<ResponseWrapper<ConfigResponse>> GetConfigLastTime(ConfigRequest request)
        {
            var response = new ResponseWrapper<ConfigResponse>();
            if (string.IsNullOrWhiteSpace(request.AppId) || string.IsNullOrWhiteSpace(request.EnvId) || string.IsNullOrWhiteSpace(request.Version))
            {
                response.Error = new ErrorResponse();
                response.Error.ErrorMessage = string.Format("{0:G}", DateTime.MinValue);
            }
            else
            {
                response.Result = new ConfigResponse
                {
                    Time = await _configRepository.GetConfigLastTime(request.AppId, request.Version, request.EnvId)
                };
            }
            return response;
        }

        public async Task<ResponseWrapper<Dictionary<int, Dictionary<string, string>>>> GetConfigs(ConfigRequest request)
        {
            var response = new ResponseWrapper<Dictionary<int, Dictionary<string, string>>>();
            if (string.IsNullOrWhiteSpace(request.AppId) || string.IsNullOrWhiteSpace(request.EnvId) || string.IsNullOrWhiteSpace(request.Version))
            {
                response.Error = new ErrorResponse();
            }
            else
            {
                var configsCache = DisconfigCache.Get(request.AppId + request.Version + request.EnvId);
                if (configsCache == null)
                {
                    var list = await _configRepository.GetConfigs(request.AppId, request.Version, request.EnvId, request.Type, request.ConfigName);
                    var responseList = PackList(list);
                    response.Result = responseList;
                    DisconfigCache.Set(request.AppId + request.Version + request.EnvId, responseList);
                }
                else
                    response.Result = (Dictionary<int, Dictionary<string, string>>)configsCache;
            }
            return response;
        }

        public async Task<ResponseWrapper<string>> GetConfig(ConfigRequest request)
        {
            var response = new ResponseWrapper<string>();
            if (string.IsNullOrWhiteSpace(request.AppId) || string.IsNullOrWhiteSpace(request.EnvId) || string.IsNullOrWhiteSpace(request.Version) || !request.Type.HasValue || string.IsNullOrWhiteSpace(request.ConfigName))
            {
                response.Error = new ErrorResponse();
            }
            else
            {
                var configsCache = DisconfigCache.Get(request.AppId + request.Version + request.EnvId);
                if (configsCache == null)
                {
                    var list = await _configRepository.GetConfigs(request.AppId, request.Version, request.EnvId, null, null);
                    var responseList = PackList(list);
                    DisconfigCache.Set(request.AppId + request.Version + request.EnvId, responseList);
                    response.Result = GetConfigValue(responseList, request.Type, request.ConfigName);
                }
                else
                    response.Result = GetConfigValue((Dictionary<int, Dictionary<string, string>>)configsCache, request.Type, request.ConfigName);
            }
            return response;
        }
        private string GetConfigValue(Dictionary<int, Dictionary<string, string>> list, int? type, string configName)
        {
            return list[type.Value][configName];
        }
        /// <summary>
        /// 组装
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Dictionary<int, Dictionary<string, string>> PackList(IEnumerable<Configs> list)
        {
            var responseList = new Dictionary<int, Dictionary<string, string>>();
            foreach (var item in list.GroupBy(s => s.Type))
            {
                var dictionary = new Dictionary<string, string>();
                foreach (var str in item)
                {
                    dictionary.Add(str.Name, str.Value);
                }
                responseList.Add(item.FirstOrDefault().Type, dictionary);
            }
            return responseList;
        }
    }
}
