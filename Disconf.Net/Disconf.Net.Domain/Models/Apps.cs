using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Models
{
    public class Apps : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
