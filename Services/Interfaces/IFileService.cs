namespace TechSupportXPress.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveAttachmentAsync(IFormFile file, string prefix = "Ticket_Attachment");
    }
}
