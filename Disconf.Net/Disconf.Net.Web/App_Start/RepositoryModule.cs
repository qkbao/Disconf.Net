using Autofac;
using Autofac.Integration.Mvc;
using Bootstrap;
using Disconf.Net.Application.Api.Zookeeper;
using Disconf.Net.Application.Implementation;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Repositories;
using Disconf.Net.Repositories;
using Disconf.Net.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disconf.Net.Web.App_Start
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterControllers(System.Reflection.Assembly.GetExecutingAssembly()).PropertiesAutowired();
            builder.RegisterFilterProvider();

            //注入Repositorie
            builder.RegisterType<BaseRepository>().As<IBaseRepository>();
            builder.RegisterType<EnvRepository>().As<IEnvRepository>();
            builder.RegisterType<AppRepository>().As<IAppRepository>();
            builder.RegisterType<TemplateRepository>().As<ITemplateRepository>();
            builder.RegisterType<ConfigRepository>().As<IConfigRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<RoleRepository>().As<IRoleRepository>();
            builder.RegisterType<PermissionRepository>().As<IPermissionRepository>();
            builder.RegisterType<LogRepository>().As<ILogRepository>();
            //注入Service
            builder.RegisterType<EnvServiceImpl>().As<IEnvService>();
            builder.RegisterType<LogServiceImpl>().As<ILogService>();
            builder.RegisterType<AppServiceImpl>().As<IAppService>();
            builder.RegisterType<TemplateServiceImpl>().As<ITemplateService>();
            builder.RegisterType<ConfigServiceImpl>().As<IConfigService>();
            builder.RegisterType<UserServiceImpl>().As<IUserService>();
            builder.RegisterType<RoleServiceImpl>().As<IRoleService>();
            builder.RegisterType<PermissionServiceImpl>().As<IPermissionService>();

            //注入属性

            var container = (IContainer)Bootstrapper.Container;
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}