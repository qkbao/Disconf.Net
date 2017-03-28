using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetList();
        Task<bool> Insert(User model);
        Task<bool> Update(User model);
        Task<User> Get(long id);
        Task<bool> Delete(long id);
        Task<User> Login(User model);
        Task<IEnumerable<User>> GetUserList(long userId);
    }
}
