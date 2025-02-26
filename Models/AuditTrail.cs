using System.ComponentModel;

namespace TechSupportXPress.Models
{
    public class AuditTrail
    {
        [DisplayName("No")]
        public int Id { get; set; }

        [DisplayName("Action Type")]
        public string Action { get; set; }

        [DisplayName("Module")]
        public string Module { get; set; }

        public string AffectedTable { get; set; }

        [DisplayName("Created On")]
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        public string IpAddress { get; set; }

        [DisplayName("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
