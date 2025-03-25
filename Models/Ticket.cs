using System.ComponentModel;

namespace TechSupportXPress.Models
{
    public class Ticket : AuditInfo
    {
        [DisplayName("No")]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }


        [DisplayName("Description")]
        public string Description { get; set; }


        [DisplayName("Status")]
        public int StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }


        [DisplayName("Priority")]
        public int PriorityId { get; set; }
        public SystemCodeDetail Priority { get; set; }


        [DisplayName("Ticket Sub-Category")]
        public int? SubCategoryId { get; set; }
        public TicketSubCategory SubCategory { get; set; }

        [DisplayName("Attachment")]
        public string? Attachment { get; set; }

        public ICollection<Comment> TicketComments { get; set; }

        public string? AssignedToId { get; set; }
        public ApplicationUser AssignedTo { get; set; }

        public DateTime? AssignedOn { get; set; }

        [DisplayName("Duration (Minutes)")]
        public int? TicketDuration
        {
            get
            {
                if (CreatedOn == null)
                    return null;

                DateTime now = DateTime.UtcNow;
                TimeSpan difference = now.Subtract(CreatedOn);

                return (int)difference.TotalMinutes;
            }
        }

    }
}
