using DripsCampaignTracker.Enums;

namespace DripsCampaignTracker.Entity;

public class Campaign
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GoalTarget { get; set; }
    public int CooldownDays { get; set; }
    public bool AutoClose { get; set; }
    public CampaignStatus Status { get; set; }
    public int MarketerId { get; set; }
    public int ManagerId { get; set; }
    public DateTime CreatedDate { get; set; }

    public Employee? Marketer { get; set; }
    public Employee? Manager { get; set; }
    public List<CampaignLead> CampaignLeads { get; set; } = [];
    public List<Conversation> Conversations { get; set; } = [];
}
