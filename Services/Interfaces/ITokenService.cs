using Ticket_System.Models;

namespace Ticket_System.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
