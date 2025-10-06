using Ticket_System.Models.Enums;

public class UpdateOwnTicketDto
{
    public int TicketId { get; set; }
    public Status Status { get; set; }
    public DateTime? DueDate { get; set; }

}
