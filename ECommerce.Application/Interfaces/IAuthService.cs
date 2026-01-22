using ECommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApplicationUser?> ValidateUserAsync(string phone, string password);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
    }
}
