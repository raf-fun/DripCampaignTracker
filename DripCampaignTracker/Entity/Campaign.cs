using DripCampaignTracker.Enums;

namespace DripCampaignTracker.Entity;

public class Campaign
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int GoalTarget { get; set; }
    public int CooldownDays { get; set; }
    public bool AutoClose { get; set; }
    public CampaignStatus Status { get; set; }
    public int MarketerId { get; set; }
    public int ManagerId { get; set; }
    public DateTime CreatedDate { get; set; }
}
