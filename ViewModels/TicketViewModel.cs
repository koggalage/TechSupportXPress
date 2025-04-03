using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TechSupportXPress.Models;

namespace TechSupportXPress.ViewModels
{
    public class TicketCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [DisplayName("Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [DisplayName("Priority")]
        public int PriorityId { get; set; }

        [DisplayName("Ticket Category")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Sub-category is required")]
        [DisplayName("Ticket Sub-Category")]
        public int? SubCategoryId { get; set; }

        public IFormFile? Attachment { get; set; }
    }

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

        public TicketResolution Resolution { get; set; }
        public List<TicketResolution> TicketResolutions { get; set; }


        [DisplayName("Assigned To")]
        public string? AssignedToId { get; set; }
        public ApplicationUser AssignedTo { get; set; }

        [DisplayName("Assigned On")]
        public DateTime? AssignedOn { get; set; }

        [DisplayName("Ticket Category")]
        public int TicketCategoryId { get; set; }

        public string NextStatus { get; set; }

    }
}
