using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllAdminUsersAsync();
        Task<List<ApplicationUser>> GetAllCustomersAsync();
        Task<ApplicationUser?> GetByPhoneAsync(string phone);
        Task AddAsync(ApplicationUser user);
    }
}
