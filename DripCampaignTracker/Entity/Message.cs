using DripCampaignTracker.Enums;

namespace DripCampaignTracker.Entity;

public class Message
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public string Content { get; set; }
    public SenderType SenderType { get; set; }
    public Classification Classification { get; set; }
    public DateTime SentDate { get; set; }

    public Conversation Conversation { get; set; }
}
