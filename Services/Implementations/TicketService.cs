using Microsoft.EntityFrameworkCore;
using Ticket_System.Data;
using Ticket_System.DTOs;
using Ticket_System.Models;
using Ticket_System.Models.Enums;
using Ticket_System.Services.Interfaces;

namespace Ticket_System.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<TicketService> _logger;  
        public TicketService(AppDbContext context, IUserService userService)//ILogger<TicketService> logger
        {
            _context = context;
            _userService = userService;
            //_logger = logger;
        }

        public async Task<object> CreateTicketAsync(CreateTicketDto dto, string userEmail)
        {
            //_logger.LogInformation("Creating ticket for user: {UserEmail}", userEmail);
            if (string.IsNullOrEmpty(userEmail))
                throw new Exception("Email not found in token");

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            int requesterId = user.UserId;

       
            if (!Enum.IsDefined(typeof(Team), dto.Team))
                throw new ArgumentException("Invalid team selected.");
            if (!Enum.IsDefined(typeof(Category), dto.Category))
                throw new ArgumentException("Invalid category.");
            if (!Enum.IsDefined(typeof(Priority), dto.Priority))
                throw new ArgumentException("Invalid priority.");

            int? assigneeId = null;

            if (dto.AssigneeId.HasValue)
            {
                var assignee = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.AssigneeId.Value);
                if (assignee == null)
                    throw new ArgumentException("Assignee not found.");

                if (assignee.Team != dto.Team)
                    throw new ArgumentException("Assignee does not belong to the selected team.");

                assigneeId = assignee.UserId;
            }

            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = (Category)dto.Category,
                Priority = (Priority)dto.Priority,
                Status = Status.Open,
                Team = (Team)dto.Team,
                RequesterId = requesterId,
                RequesterName = user.Username,
                CreatedDate = DateTime.UtcNow,
                DueDate = dto.DueDate,
                AssigneeId = assigneeId
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return new { ticket.TicketId, Message = "Ticket created successfully." };
        }

        public async Task<List<TicketSummaryDto>> GetAllTicketsForAdminAsync(string userEmail)
        {
            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null || user.Role != "Admin")
                throw new UnauthorizedAccessException("You are not allowed to view all tickets other than those assigned to your team.");

            var tickets = await _context.Tickets
                .Include(t => t.Requester)
                .Include(t => t.Assignee)
                .Select(t => new TicketSummaryDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Category = t.Category.ToString(),
                    Team = t.Team.ToString(),
                    RequesterName = t.Requester.Username,
                    AssigneeName = t.Assignee != null ? t.Assignee.Username : null,
                    CreatedDate = t.CreatedDate.ToString("yyyy-MM-dd"),
                    DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("yyyy-MM-dd") : null
                })
                .ToListAsync();

            return tickets;
        }

        public async Task<TicketDetailsDto> GetTicketByIdAsync(int id, string userEmail, string userRole)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new Exception("Email not found in token");

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            var userId = user.UserId;

            var ticket = await _context.Tickets
                .Include(t => t.Requester)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            var ticketDto = new TicketDetailsDto
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                Category = ticket.Category.ToString(),
                Team = ticket.Team.ToString(),
                RequesterId = ticket.RequesterId,
                RequesterName = ticket.Requester?.Username,
                AssigneeId = ticket.AssigneeId,
                AssigneeName = ticket.Assignee?.Username,
                CreatedDate = ticket.CreatedDate.ToString("yyyy-MM-dd HH:mm"),
                DueDate = ticket.DueDate?.ToString("yyyy-MM-dd HH:mm"),
                ResolvedDate = ticket.ResolvedDate?.ToString("yyyy-MM-dd HH:mm")
            };

            if (ticket.RequesterId == userId)
                return ticketDto;

            if (ticket.Team == user.Team)
                return ticketDto;

            if (userRole == "Admin")
                return ticketDto;

            throw new UnauthorizedAccessException("You are not authorized to view this ticket.");
        }

        public async Task<List<object>> GetTicketsByUserAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new UnauthorizedAccessException("Email not found in token.");

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var tickets = await _context.Tickets
                .Where(t => t.RequesterId == user.UserId)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .ToListAsync();

            var result = tickets.Select(ticket => new
            {
                ticket.TicketId,
                ticket.Title,
                ticket.Description,
                ticket.Category,
                ticket.Priority,
                ticket.Status,
                ticket.Team,
                ticket.DueDate,
                ticket.CreatedDate,
                ticket.ResolvedDate,
                AssigneeName = ticket.Assignee?.Username,
                RequesterName = ticket.Requester?.Username
            }).ToList<object>();

            return result;
        }

        public async Task<List<object>> GetTicketsForTeamAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new UnauthorizedAccessException("Email not found in token.");

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var tickets = await _context.Tickets
                .Where(t => t.Team == user.Team)
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .ToListAsync();

            var result = tickets.Select(ticket => new
            {
                ticket.TicketId,
                ticket.Title,
                ticket.Description,
                ticket.Category,
                ticket.Priority,
                ticket.Status,
                ticket.Team,
                DueDate = ticket.DueDate?.ToString("yyyy-MM-dd HH:mm"),
                CreatedDate = ticket.CreatedDate.ToString("yyyy-MM-dd HH:mm"),
                ResolvedDate = ticket.ResolvedDate?.ToString("yyyy-MM-dd HH:mm"),
                AssigneeName = ticket.Assignee?.Username,
                RequesterName = ticket.Requester?.Username
            }).ToList<object>();

            return result;
        }

        public async Task<List<object>> GetTicketStatusesAsync(string userEmail)
        {
            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            IQueryable<Ticket> query;

            if (user.Role == "Admin")
            {
                query = _context.Tickets;
            }
            else
            {
                query = _context.Tickets
                    .Where(t => t.RequesterId == user.UserId || t.AssigneeId == user.UserId);
            }

            var ticketStatuses = await query
                .Select(t => new
                {
                    t.TicketId,
                    t.Title,
                    t.Status
                })
                .ToListAsync();

            return ticketStatuses.Cast<object>().ToList();
        }

        public async Task<object> UpdateOwnTicketAsync(UpdateOwnTicketDto dto, string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new UnauthorizedAccessException("Email not found in token.");

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t =>
                t.TicketId == dto.TicketId &&
                (t.AssigneeId == user.UserId || t.RequesterId == user.UserId));

            if (ticket == null)
                throw new UnauthorizedAccessException("You can only update tickets assigned to you or requested by you.");

            ticket.Status = dto.Status;

            if (dto.DueDate.HasValue)
                ticket.DueDate = dto.DueDate.Value.Date;

            if (dto.Status == Status.Resolved || dto.Status == Status.Closed)
                ticket.ResolvedDate = DateTime.UtcNow.Date;

            await _context.SaveChangesAsync();

            return new
            {
                ticket.TicketId,
                ticket.Status,
                DueDate = ticket.DueDate?.ToString("yyyy-MM-dd"),
                CreatedDate = ticket.CreatedDate.ToString("yyyy-MM-dd"),
                ResolvedDate = ticket.ResolvedDate?.ToString("yyyy-MM-dd"),
                Message = "Ticket updated successfully."
            };
        }

        public async Task<object> AdminUpdateTicketAsync(UpdateOwnTicketDto dto, string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new UnauthorizedAccessException("Email not found in token.");

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");
            if (user.Role != "Admin")
                throw new UnauthorizedAccessException("You are not authorized to perform this action.");

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == dto.TicketId);
            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            ticket.Status = dto.Status;

            if (dto.DueDate.HasValue)
                ticket.DueDate = dto.DueDate.Value.Date;

            if (dto.Status == Status.Resolved || dto.Status == Status.Closed)
                ticket.ResolvedDate = DateTime.UtcNow.Date;

            await _context.SaveChangesAsync();

            return new
            {
                ticket.TicketId,
                ticket.Status,
                DueDate = ticket.DueDate?.ToString("yyyy-MM-dd"),
                CreatedDate = ticket.CreatedDate.ToString("yyyy-MM-dd"),
                ResolvedDate = ticket.ResolvedDate?.ToString("yyyy-MM-dd"),
                Message = "Ticket updated successfully by Admin."
            };
        }

        public async Task<string> UpdateTicketStatusAsync(int id, UpdateStatusDto dto, string userEmail)
        {
            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            var ticket = await _context.Tickets
                .Include(t => t.Requester)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found");

            if (ticket.Status == Status.Closed)
                throw new ArgumentException("Ticket is already closed and cannot be modified.");

            if (user.Role == "Admin")
            {
                ticket.Status = dto.Status;
                ticket.ResolvedDate = dto.Status == Status.Closed ? DateTime.UtcNow : null;
                await _context.SaveChangesAsync();
                return "Ticket status updated by Admin.";
            }

            if (ticket.AssigneeId.HasValue)
            {
                if (ticket.AssigneeId != user.UserId)
                    throw new UnauthorizedAccessException("Only the assigned user can update the ticket status.");
            }
            else
            {
                if (ticket.Team != user.Team)
                    throw new UnauthorizedAccessException("Only a member of the assigned team can update the ticket status.");
            }

            ticket.Status = dto.Status;
            ticket.ResolvedDate = dto.Status == Status.Closed ? DateTime.UtcNow : null;
            await _context.SaveChangesAsync();

            return "Ticket status updated.";
        }


        public async Task<List<object>> GetAssignedTicketsByUserAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new UnauthorizedAccessException("Email not found in token.");

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var tickets = await _context.Tickets
                .Where(t => t.AssigneeId == user.UserId)   
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .ToListAsync();

            return tickets.Select(ticket => new
            {
                ticket.TicketId,
                ticket.Title,
                ticket.Description,
                ticket.Category,
                ticket.Priority,
                ticket.Status,
                ticket.Team,
                DueDate = ticket.DueDate?.ToString("yyyy-MM-dd HH:mm"),
                CreatedDate = ticket.CreatedDate.ToString("yyyy-MM-dd HH:mm"),
                ResolvedDate = ticket.ResolvedDate?.ToString("yyyy-MM-dd HH:mm"),
                AssigneeId = ticket.AssigneeId,
                AssigneeName = ticket.Assignee?.Username,
                RequesterId = ticket.RequesterId,
                RequesterName = ticket.Requester?.Username
            }).Cast<object>().ToList();

        }

        public async Task<List<object>> GetRequestedTicketsByUserAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new UnauthorizedAccessException("Email not found in token.");

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var tickets = await _context.Tickets
                .Where(t => t.RequesterId == user.UserId)  
                .Include(t => t.Assignee)
                .Include(t => t.Requester)
                .ToListAsync();

            return tickets.Select(ticket => new
            {
                ticket.TicketId,
                ticket.Title,
                ticket.Description,
                ticket.Category,
                ticket.Priority,
                ticket.Status,
                ticket.Team,
                DueDate = ticket.DueDate?.ToString("yyyy-MM-dd HH:mm"),
                CreatedDate = ticket.CreatedDate.ToString("yyyy-MM-dd HH:mm"),
                ResolvedDate = ticket.ResolvedDate?.ToString("yyyy-MM-dd HH:mm"),
                AssigneeId = ticket.AssigneeId,
                AssigneeName = ticket.Assignee?.Username,
                RequesterId = ticket.RequesterId,
                RequesterName = ticket.Requester?.Username
            }).Cast<object>().ToList();
        }

    }
}