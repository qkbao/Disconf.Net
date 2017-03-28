using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Autofac.Extras.DynamicProxy2;
using Autofac.Integration.WebApi;
using Autofac;
using System.Web.Configuration;
using Disconf.Net.Repositories;
using Disconf.Net.Application;
using Disconf.Net.Application.Api;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Application.Implementation;
using Disconf.Net.Repositories.Repository;
using Disconf.Net.Domain.Repositories;

namespace Disconf.Net.Web.App_Start
{
    public class DependencyRegister
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            // Call RegisterHttpRequestMessage to add the feature.
            builder.RegisterHttpRequestMessage(GlobalConfiguration.Configuration);

            // Register your Web API controllers.
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

            var connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            builder.RegisterAssemblyTypes(typeof(BaseRepository).Assembly)
                .Where(t => t.IsSubclassOf(typeof(BaseRepository)))
                .AsImplementedInterfaces()
                .WithParameter((new TypedParameter(typeof(string), connectionString)))
                .EnableInterfaceInterceptors();

            builder.RegisterAssemblyTypes(typeof(IRequest<>).Assembly)
                .Where(t => t.Name.EndsWith("Request"))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly)
                .Where(t => t.Name.EndsWith("Impl"))
                .AsImplementedInterfaces();
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}