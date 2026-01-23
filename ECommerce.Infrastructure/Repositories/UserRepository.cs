using ECommerce.Domain;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByPhoneAsync(string phone)
            => await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);

        public async Task AddAsync(ApplicationUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ApplicationUser>> GetAllCustomersAsync()
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");

            return customers.ToList();
        }

        public async Task<List<ApplicationUser>> GetAllAdminUsersAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            return admins.ToList();
        }
    }

}
