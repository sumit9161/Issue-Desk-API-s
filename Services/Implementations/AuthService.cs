using BCrypt.Net;
using Ticket_System.Data;
using Ticket_System.DTOs;
using Ticket_System.Models;
using Ticket_System.Models.Enums;
using Ticket_System.Services.Interfaces;
using System.Dynamic;

namespace Ticket_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<object> RegisterAsync(RegisterUserDto request, bool isAuthenticated, bool isAdmin)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
                throw new ArgumentException("User with this email already exists.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            bool isFirstAdmin = !_context.Users.Any(u => u.Role == "Admin");

            if (request.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                if (!isFirstAdmin)
                    throw new UnauthorizedAccessException("Admin already exists. Please login to create more users.");

                var adminUser = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    Role = "Admin",
                    Team = null
                };

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();

                var token = _tokenService.CreateToken(adminUser);

                dynamic user = new ExpandoObject();
                user.Username = adminUser.Username;
                user.Email = adminUser.Email;
                user.Role = adminUser.Role;

                dynamic response = new ExpandoObject();
                response.message = "Admin registered successfully.";
                response.token = token;
                response.user = user;

                return response;
            }

            if (!isAuthenticated || !isAdmin)
                throw new UnauthorizedAccessException("Only Admins can register users.");

            if (string.IsNullOrWhiteSpace(request.Team))
                throw new ArgumentException("Team is required for user registration.");

            if (!Enum.TryParse<Team>(request.Team, true, out var parsedTeam))
                throw new ArgumentException("Invalid team. Valid values: DevOps, QA, Developer, HR, ITSupport");

            var normalUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "User",
                Team = parsedTeam
            };

            _context.Users.Add(normalUser);
            await _context.SaveChangesAsync();

            dynamic userObj = new ExpandoObject();
            userObj.Username = normalUser.Username;
            userObj.Email = normalUser.Email;
            userObj.Role = normalUser.Role;
            userObj.Team = normalUser.Team.ToString();

            dynamic successResponse = new ExpandoObject();
            successResponse.message = "User registered successfully.";
            successResponse.user = userObj;

            return successResponse;
        }

        public LoginResponse LoginAsync(LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _tokenService.CreateToken(user);

            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role,
                Id = user.UserId,
                Team = user.Team.ToString()
            };
        }
    }
}