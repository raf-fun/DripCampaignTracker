using DripsCampaignTracker.Enums;

namespace DripsCampaignTracker.Entity;

public class Message
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public SenderType SenderType { get; set; }
    public Classification Classification { get; set; }
    public DateTime SentDate { get; set; }

    public Conversation? Conversation { get; set; }
}
