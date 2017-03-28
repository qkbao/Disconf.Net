using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<IEnumerable<Templates>> GetList(TemplateCondition condition);
        Task<bool> Insert(Templates model);
        Task<bool> Update(Templates model);
        Task<Templates> Get(long id);
        Task<bool> Delete(long id);
        Task<IEnumerable<Templates>> GetTemplateList(TemplateCondition condition);
        Task<bool> BatchCreateFile(TemplateCondition condition);
    }
}
