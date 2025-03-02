using System.ComponentModel;
using TechSupportXPress.Models;

namespace TechSupportXPress.ViewModels
{
    public class TicketViewModel : AuditInfo
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


        [DisplayName("Ticket Category")]
        public int? CategoryId { get; set; }

        [DisplayName("Ticket Sub-Category")]
        public int? SubCategoryId { get; set; }
        public TicketSubCategory SubCategory { get; set; }

        public List<Ticket> Tickets { get; set; }

        [DisplayName("Attachment")]
        public string Attachment { get; set; }

        public Comment TicketComment { get; set; }

        public List<Comment> TicketComments { get; set; }
        public Ticket TicketDetails { get; set; }

        [DisplayName("Comment Description")]
        public string CommentDescription { get; set; }

    }
}
