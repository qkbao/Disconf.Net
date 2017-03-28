using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Repositories
{
    public interface ITemplateRepository : IBaseRepository
    {
        Task<IEnumerable<Templates>> GetTemplateList(TemplateCondition condition);
    }
}
