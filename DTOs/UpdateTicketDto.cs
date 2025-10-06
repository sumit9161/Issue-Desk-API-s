using Ticket_System.Models.Enums;

public class UpdateTicketDto
{
    // Admin-only fields
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Category? Category { get; set; }
    public Priority? Priority { get; set; }
    public Team? Team { get; set; }
    public int? AssigneeId { get; set; }
    public Status Status { get; set; }
    public DateTime? DueDate { get; set; }

    public string? Comment { get; set; }
}
