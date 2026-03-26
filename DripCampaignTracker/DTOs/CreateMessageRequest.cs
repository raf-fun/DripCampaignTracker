using DripCampaignTracker.Enums;

namespace DripCampaignTracker.DTOs;

public class CreateMessageRequest
{
    public int ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public SenderType SenderType { get; set; }
}
