using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ticket_System.Models.Enums;

namespace Ticket_System.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public Priority Priority { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public Team Team { get; set; }

        [Required]
        public int RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public User Requester { get; set; }

        [ForeignKey("Assignee")]
        public int? AssigneeId { get; set; }

        public User? Assignee { get; set; }

        public string? RequesterName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
    }
}
