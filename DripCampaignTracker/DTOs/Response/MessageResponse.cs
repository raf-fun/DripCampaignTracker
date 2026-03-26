using DripsCampaignTracker.Enums;

namespace DripsCampaignTracker.DTOs.Response;

public class MessageResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public SenderType SenderType { get; set; }
    public Classification Classification { get; set; }
    public DateTime SentDate { get; set; }
}
