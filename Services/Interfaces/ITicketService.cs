using Ticket_System.DTOs;
using Ticket_System.Models.Enums;

namespace Ticket_System.Services.Interfaces
{
    public interface ITicketService
    {
        Task<object> CreateTicketAsync(CreateTicketDto dto, string userEmail);
        Task<List<TicketSummaryDto>> GetAllTicketsForAdminAsync(string userEmail);
        Task<TicketDetailsDto> GetTicketByIdAsync(int id, string userEmail, string userRole);
        Task<List<object>> GetTicketsByUserAsync(string userEmail);
        Task<List<object>> GetTicketsForTeamAsync(string userEmail);
        Task<List<object>> GetTicketStatusesAsync(string userEmail);
        Task<object> UpdateOwnTicketAsync(UpdateOwnTicketDto dto, string userEmail);
        Task<object> AdminUpdateTicketAsync(UpdateOwnTicketDto dto, string userEmail);
        Task<string> UpdateTicketStatusAsync(int id, UpdateStatusDto dto, string userEmail);
        Task<List<object>> GetAssignedTicketsByUserAsync(string userEmail);
        Task<List<object>> GetRequestedTicketsByUserAsync(string userEmail);

    }
}