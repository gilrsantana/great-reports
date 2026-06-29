namespace GreatReports.Infrastructure.Configurations;

public class MailProviderSettings
{
    public const string SectionName = "MailProviderSettings";
    public string BaseAddress { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string ManagerApiKey { get; set; } = string.Empty;
    public string SenderApiKey { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
}
