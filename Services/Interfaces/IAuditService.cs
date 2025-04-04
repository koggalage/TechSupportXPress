namespace TechSupportXPress.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string action, string userId, string module, string table, string ipAddress);
    }
}
