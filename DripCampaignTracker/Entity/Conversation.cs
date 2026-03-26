using DripCampaignTracker.Enums;

namespace DripCampaignTracker.Entity;

public class Conversation
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int LeadId { get; set; }
    public ConversationStatus Status { get; set; }
    public int FollowUpCount { get; set; }
    public DateTime LastContactedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
