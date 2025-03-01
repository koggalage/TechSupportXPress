namespace TechSupportXPress.Models
{
    public class SystemCode : AuditInfo
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}
