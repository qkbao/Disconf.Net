using DapperExtensions;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using Disconf.Net.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Implementation
{
    public class TemplateServiceImpl : ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;
        public TemplateServiceImpl(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }
        public async Task<IEnumerable<Templates>> GetList(TemplateCondition condition)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            if (condition.AppId.HasValue)
            {
                pg.Predicates.Add(Predicates.Field<Templates>(l => l.AppId, Operator.Eq, condition.AppId.Value));
            }
            return await _templateRepository.GetListWithCondition<Templates>(pg);
        }

        public async Task<bool> Insert(Templates model)
        {
            return await _templateRepository.Insert(model);
        }
        public async Task<bool> Update(Templates model)
        {
            return await _templateRepository.Update(model);
        }

        public async Task<Templates> Get(long id)
        {
            return await _templateRepository.GetById<Templates>(id);
        }

        public async Task<bool> Delete(long id)
        {
            return await _templateRepository.Delete<Templates>(id);
        }

        public async Task<IEnumerable<Templates>> GetTemplateList(TemplateCondition condition)
        {
            return await _templateRepository.GetTemplateList(condition);
        }


        public async Task<bool> BatchCreateFile(TemplateCondition condition)
        {
            var list = await GetTemplateList(condition);
            FormatFolder(AppSettingHelper.Get<string>("FilePath"));
            FormatFolder(AppSettingHelper.Get<string>("ZipPath"));
            foreach (var item in list)
            {
                FileStream myFs = new FileStream(AppSettingHelper.Get<string>("FilePath") + item.Name, FileMode.Create);
                StreamWriter mySw = new StreamWriter(myFs);
                if (string.IsNullOrWhiteSpace(item.ConfigValue))
                    mySw.Write(item.DefaultValue);
                else
                    mySw.Write(item.ConfigValue);
                mySw.Close();
                myFs.Close();
            }
            return true;
        }
        private void FormatFolder(string path)
        {
            FileInfo fi = new FileInfo(path);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo f in dir.GetFiles("*.*"))
                    f.Delete();
            }
        }
    }
}
