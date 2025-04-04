using System.ComponentModel;

namespace TechSupportXPress.Models
{
    public class TicketCategory : AuditInfo
    {
        [DisplayName("Category No")]
        public int Id { get; set; }

        [DisplayName("Category Code")]
        public string Code { get; set; }

        [DisplayName("Category Name")]

        public string Name { get; set; }
    }
}
