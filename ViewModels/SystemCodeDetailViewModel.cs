using System.ComponentModel;
using TechSupportXPress.Models;

namespace TechSupportXPress.ViewModels
{
    public class SystemCodeDetailViewModel : AuditInfo
    {
        public int Id { get; set; }

        [DisplayName("Code")]
        public string Code { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }


        [DisplayName("Order No")]
        public int? OrderNo { get; set; }


        [DisplayName("System Code")]
        public int SystemCodeId { get; set; }
        public SystemCode SystemCode { get; set; }

        public List<SystemCodeDetail> SystemCodeDetails { get; set; }
    }
}
