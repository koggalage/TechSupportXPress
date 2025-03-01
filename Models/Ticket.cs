﻿using System.ComponentModel;

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
        public string Status { get; set; }

        [DisplayName("Priority")]
        public string Priority { get; set; }

        [DisplayName("Ticket Sub-Category")]
        public int? SubCategoryId { get; set; }
        public TicketSubCategory SubCategory { get; set; }

    }
}
