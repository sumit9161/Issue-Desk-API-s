namespace Ticket_System.Models.Configuration
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int ExpireMinutes { get; set; }
    }
}
