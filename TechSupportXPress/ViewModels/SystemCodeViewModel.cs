using System.ComponentModel;
using TechSupportXPress.Models;

namespace TechSupportXPress.ViewModels
{
    public class SystemCodeViewModel : AuditInfo
    {

        [DisplayName("No")]
        public int Id { get; set; }

        [DisplayName("System Code")]
        public string Code { get; set; }


        [DisplayName("Description")]
        public string Description { get; set; }

        public List<SystemCode> SystemCodes { get; set; }
    }
}
