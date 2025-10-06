using Ticket_System.Models.Enums;

public class UpdateAssignedTicketDto
{
    public Status Status { get; set; }
    public DateTime? DueDate { get; set; }
}
