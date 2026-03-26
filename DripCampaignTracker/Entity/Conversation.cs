using DripsCampaignTracker.Enums;
using System.Collections.Generic;

namespace DripsCampaignTracker.Entity;

public class Conversation
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int LeadId { get; set; }
    public ConversationStatus Status { get; set; }
    public int FollowUpCount { get; set; }
    public DateTime LastContactedDate { get; set; }
    public DateTime CreatedDate { get; set; }

    public Campaign? Campaign { get; set; }
    public Lead? Lead { get; set; }
    public List<Message> Messages { get; set; } = [];
}
