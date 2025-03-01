﻿namespace TechSupportXPress.Models
{
    public class SystemCodeDetail : AuditInfo
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public int? OrderNo { get; set; }

        public int SystemCodeId { get; set; }
        public SystemCode SystemCode { get; set; }
    }
}
