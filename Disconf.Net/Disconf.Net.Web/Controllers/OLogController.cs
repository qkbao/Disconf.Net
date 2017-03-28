using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Models;
using Disconf.Net.Infrastructure.Helper;
using Disconf.Net.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace Disconf.Net.Web.Controllers
{
    public class OLogController : BaseController
    {
        // GET: OLog
        private readonly ILogService _logService;
        private readonly IUserService _userService;
        public OLogController(ILogService logService, IUserService userService)
        {
            this._logService = logService;
            this._userService = userService;
        }
        public async Task<ActionResult> Index(LogPagingFilteringVM command)
        {
            if (command == null)
                command = new LogPagingFilteringVM();
            command.Content = command.Content.RemoveSpace();
            LogListVM model = new LogListVM()
            {
                Logs = await this.GetLogPageList(command),
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                Content = command.Content
            };
            return View(model);
        }

        private async Task<IPagedList<OperationLog>> GetLogPageList(LogPagingFilteringVM command)
        {
            var total = await this._logService.GetLogTotal(command);
            var users = await this._userService.GetList();
            var userDic = users.ToDictionary(s => s.Id, v => v.Name);
            IEnumerable<OperationLog> logs;
            if (total > 0)
                logs = await this._logService.GetLogList(PageSize, command);
            else
                logs = new List<OperationLog>();
            logs.ToList().ForEach(s =>
            {
                s.Name = userDic[s.UId];
            });
            return new StaticPagedList<OperationLog>(logs, command.Page, PageSize, total);
        }
    }
}