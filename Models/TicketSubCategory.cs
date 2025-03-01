namespace TechSupportXPress.Models
{
    public class TicketSubCategory : AuditInfo
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public TicketCategory Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
