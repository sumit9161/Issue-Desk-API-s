using Ticket_System.Models.Enums;

namespace Ticket_System.DTOs
{
    public class TicketDto
    {
        public int TicketId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }
        public Category Category { get; set; }
        public Team TeamAssigned { get; set; }
        public int RequesterId { get; set; }
        public int? AssigneeId { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
