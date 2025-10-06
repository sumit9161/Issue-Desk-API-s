using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ticket_System.Models.Enums;

namespace Ticket_System.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; }
        public Team? Team { get; set; }
        [InverseProperty(nameof(Ticket.Requester))]
        public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();

        [InverseProperty(nameof(Ticket.Assignee))]
        public ICollection<Ticket> ClaimedTickets { get; set; } = new List<Ticket>();
    }
}
