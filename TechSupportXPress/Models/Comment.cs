using System.ComponentModel;

namespace TechSupportXPress.Models
{
    public class Comment : AuditInfo
    {
        public int Id { get; set; }

        public string Description { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
