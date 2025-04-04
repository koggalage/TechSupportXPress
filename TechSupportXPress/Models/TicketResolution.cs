namespace TechSupportXPress.Models
{
    public class TicketResolution : AuditInfo
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public Ticket Ticket { get; set; }

        public string Description { get; set; }

        public int StatusId { get; set; }

        public SystemCodeDetail Status { get; set; }
    }
}
