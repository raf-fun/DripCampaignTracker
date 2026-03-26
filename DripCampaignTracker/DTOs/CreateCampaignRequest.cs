namespace DripCampaignTracker.DTOs;

public class CreateCampaignRequest
{
    public string Name { get; set; } = string.Empty;
    public int GoalTarget { get; set; }
    public int CooldownDays { get; set; }
    public bool AutoClose { get; set; }
    public int MarketerId { get; set; }
    public int ManagerId { get; set; }
}
