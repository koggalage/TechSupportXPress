using System.ComponentModel;

namespace TechSupportXPress.Models
{
    public class AuditInfo
    {
        [DisplayName("Created By")]
        public string CreatedById { get; set; }
        [DisplayName("Created By")]
        public ApplicationUser CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Modified By")]
        public string? ModifiedById { get; set; }
        public ApplicationUser ModifiedBy { get; set; }


        [DisplayName("Modified On")]
        public DateTime? ModifiedOn { get; set; }

        [DisplayName("Deleted By")]
        public string? DeletedById { get; set; }
        public ApplicationUser DeletedBy { get; set; }


        [DisplayName("Deleted On")]
        public DateTime? DeletedOn { get; set; }
    }
}
