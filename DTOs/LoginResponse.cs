namespace Ticket_System.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        public int Id { get; set; }
        public string Team { get; set; }
    }
}
