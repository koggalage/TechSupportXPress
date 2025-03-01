using System.ComponentModel;

namespace TechSupportXPress.Models
{
    public class Department : AuditInfo
    {

        [DisplayName("No")]
        public int Id { get; set; }


        [DisplayName("Department Code")]
        public string Code { get; set; }


        [DisplayName("Department Name")]
        public string Name { get; set; }
    }
}
