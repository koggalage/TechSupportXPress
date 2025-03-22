namespace TechSupportXPress.ViewModels
{
    public class UserWithRoleViewModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public string Id { get; set; } // still needed for Edit/Delete
    }
}
