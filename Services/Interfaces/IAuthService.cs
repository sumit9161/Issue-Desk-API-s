using Ticket_System.DTOs;

namespace Ticket_System.Services.Interfaces
{
    public interface IAuthService
    {
        Task<object> RegisterAsync(RegisterUserDto request, bool isAuthenticated, bool isAdmin);
        LoginResponse LoginAsync(LoginRequest request);


    }
}