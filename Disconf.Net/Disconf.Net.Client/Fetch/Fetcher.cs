using Disconf.Net.Core.Model;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Disconf.Net.Client.Fetch
{
    public class Fetcher : IFetcher
    {
        const string GetConfigResource = "/api/get";
        const string GetAllConfigsResource = "/api/getall";
        const string GetZkHostsResource = "/api/getzk";

        private string _apiHost;
        private RetryPolicy _policy;

        public Fetcher(string apiHost, RetryPolicy policy)
        {
            this._apiHost = apiHost;
            this._policy = policy;
        }
        public string GetConfig(ConfigFetchFilter filter)
        {
            return this.CallApi(GetConfigResource, request => request.AddJsonBody(filter));
        }
        public string GetAllConfigs(FetchFilter filter)
        {
            return this.CallApi(GetAllConfigsResource, request =>
            {
                request.AddJsonBody(filter);
                request.Timeout = 300000;
            });
        }
        public string GetZkHosts()
        {
            return this.CallApi(GetZkHostsResource);
        }
        private IRestResponse CallApi(IRestRequest request)
        {
            Func<IRestResponse> func = () =>
            {
                RestClient client = new RestClient(this._apiHost);
                var res = client.Execute(request);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Fetch Fail");
                }
                return res;
            };
            IRestResponse response = null;
            //如果CallApi失败，抛出的异常由外部调用方，即ConfigManager来处理，该部分不与Warn产生交集
            if (this._policy != null)
            {
                response = this._policy.ExecuteAction(func);
            }
            else
            {
                response = func();
            }
            return response;
        }

        private string CallApi(string resource, Action<IRestRequest> act = null, Method method = Method.POST)
        {
            IRestRequest request = new RestRequest(resource, method);
            if (act != null)
            {
                act(request);
            }
            var response = this.CallApi(request);
            return response.Content;
        }
    }
}
