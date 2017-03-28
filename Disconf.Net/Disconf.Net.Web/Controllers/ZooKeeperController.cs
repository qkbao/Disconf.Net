using Disconf.Net.Application.Api.App;
using Disconf.Net.Application.Api.Config;
using Disconf.Net.Application.Api.Zookeeper;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Core.Model;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Infrastructure.Filters;
using Disconf.Net.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Disconf.Net.Web.Controllers
{
    public class ZooKeeperController : ApiController
    {
        private readonly IZookeeperService _zookeeperService;
        private readonly IAppApiService _appService;
        private readonly IConfigApiService _configService;
        private readonly IEnvService _envService;
        public ZooKeeperController(IZookeeperService zookeeperService, IAppApiService appService, IConfigApiService configService, IEnvService envService)
        {
            this._zookeeperService = zookeeperService;
            this._appService = appService;
            this._configService = configService;
            this._envService = envService;
        }
        [HttpPost]
        public HttpResponseMessage GetZookeeperHost()
        {
            var host = AppSettingHelper.Get<string>("ZookeeperHost");
            return new HttpResponseMessage { Content = new StringContent(host, Encoding.GetEncoding("UTF-8"), "text/plain") };
        }
        [HttpPost]
        [CheckModelForNull]
        public async Task<IHttpActionResult> GetAppIdByName([FromBody]AppRequest request)
        {
            var response = await _appService.GetAppIdByName(request);
            if (response.HasError)
            {
                return Json(response.Error);
            }
            else
            {
                return Json(response.Result);
            }
        }

        [HttpPost]
        [CheckModelForNull]
        public async Task<HttpResponseMessage> GetConfigLastTime([FromBody]ConfigFetchFilter request)
        {
            var condition = new ConfigRequest
            {
                AppId = await GetAppId(request.AppName),
                Version = request.Version,
                EnvId = await GetEnvId(request.Environment)
            };
            var response = await _configService.GetConfigLastTime(condition);
            if (response.HasError)
            {
                return new HttpResponseMessage { Content = new StringContent(response.Error.ErrorMessage, Encoding.GetEncoding("UTF-8"), "text/plain") };
            }
            else
            {
                return new HttpResponseMessage { Content = new StringContent(string.Format("{0:G}", response.Result.Time), Encoding.GetEncoding("UTF-8"), "text/plain") };
            }
        }
        [HttpPost]
        [HttpGet]
        //[CheckModelForNull]
        public async Task<IHttpActionResult> GetConfigs([FromBody]FetchFilter request)
        {
            var condition = new ConfigRequest
            {
                AppId = await GetAppId(request.AppName),
                EnvId = await GetEnvId(request.Environment),
                Version = request.Version,
            };
            var response = await _configService.GetConfigs(condition);
            if (response.HasError)
            {
                return Json(response.Error);
            }
            else
            {
                return Json(response.Result);
            }
        }

        [HttpGet]
        [HttpPost]
        [CheckModelForNull]
        public async Task<HttpResponseMessage> GetConfig([FromBody]ConfigFetchFilter request)
        {
            var condition = new ConfigRequest
            {
                AppId = await GetAppId(request.AppName),
                EnvId = await GetEnvId(request.Environment),
                Version = request.Version,
                Type = (int)request.ConfigType,
                ConfigName = request.ConfigName
            };
            var response = await _configService.GetConfig(condition);
            if (response.HasError)
            {
                return new HttpResponseMessage { Content = new StringContent(response.Error.ErrorMessage, Encoding.GetEncoding("UTF-8"), "text/plain") };
            }
            else
            {
                return new HttpResponseMessage { Content = new StringContent(response.Result, Encoding.GetEncoding("UTF-8"), "text/plain") };
            }
        }


        public async Task<string> GetAppId(string name)
        {
            if (App == null)
                App = new Dictionary<string, string>();
            if (!App.ContainsKey(name))
            {
                var request = new AppRequest { Name = name };
                var response = await _appService.GetAppIdByName(request);
                App.Add(name, response.Result.AppId);
            }
            return App[name];
        }

        public async Task<string> GetEnvId(string name)
        {
            if (Env == null)
                Env = new Dictionary<string, string>();
            if (!Env.ContainsKey(name))
            {
                var condition = new EnvCondition { Name = name };
                var list = await _envService.GetList(condition);
                if (list == null || list.Count() == 0)
                {
                    return string.Empty;
                }
                Env.Add(name, list.FirstOrDefault().Id.ToString());
            }
            return Env[name];
        }

        public static Dictionary<string, string> App { get; set; }
        public static Dictionary<string, string> Env { get; set; }
    }
}
