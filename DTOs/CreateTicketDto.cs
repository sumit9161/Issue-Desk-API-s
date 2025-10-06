using Ticket_System.Models.Enums;

public class CreateTicketDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
    public Priority Priority { get; set; }
    public Team Team { get; set; }
    public DateTime? DueDate { get; set; }

    public int? AssigneeId { get; set; }
}
