using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application
{
    public interface IRequest<out T> where T : AbstractResponse
    {
    }
}
