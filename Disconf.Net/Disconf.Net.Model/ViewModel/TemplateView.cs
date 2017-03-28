using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Model.ViewModel
{
    public class TemplateView
    {
        public string Version { get; set; }
        public List<Templates> List { get; set; }
    }
}
