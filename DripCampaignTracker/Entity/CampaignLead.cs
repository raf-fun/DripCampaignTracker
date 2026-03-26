namespace DripCampaignTracker.Entity;

public class CampaignLead
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int LeadId { get; set; }

    public Campaign? Campaign { get; set; }
    public Lead? Lead { get; set; }
}
