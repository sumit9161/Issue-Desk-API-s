using Ticket_System.Models;
using Ticket_System.Models.Enums;

namespace Ticket_System.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<object>> GetUsersByTeamAsync(Team team);
    }
}