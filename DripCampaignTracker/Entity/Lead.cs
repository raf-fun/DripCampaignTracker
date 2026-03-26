namespace DripCampaignTracker.Entity;

public class Lead
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedDate { get; set; }

    public ICollection<CampaignLead> CampaignLeads { get; set; } = [];
    public ICollection<Conversation> Conversations { get; set; } = [];
}
