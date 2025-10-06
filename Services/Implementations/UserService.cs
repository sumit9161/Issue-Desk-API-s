using Microsoft.EntityFrameworkCore;
using Ticket_System.Data;
using Ticket_System.Models;
using Ticket_System.Models.Enums;
using Ticket_System.Services.Interfaces;

namespace Ticket_System.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<object>> GetUsersByTeamAsync(Team team)
        {
            var users = await _context.Users
                .Where(u => u.Team == team)
                .Select(u => new {
                    u.UserId,
                    u.Username
                })
                .ToListAsync();

            return users.Cast<object>().ToList();
        }
    }
}