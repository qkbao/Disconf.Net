using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.App
{
    [DataContract]
    public class AppRequest : IRequest<AppResponse>
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
