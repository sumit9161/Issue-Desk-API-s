public class TicketDetailsDto
{
    public int TicketId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public string Priority { get; set; }
    public string Status { get; set; }
    public string Category { get; set; }
    public string Team { get; set; }

    public int RequesterId { get; set; }
    public string? RequesterName { get; set; }

    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }

    public string CreatedDate { get; set; }
    public string? DueDate { get; set; }
    public string? ResolvedDate { get; set; }
}
