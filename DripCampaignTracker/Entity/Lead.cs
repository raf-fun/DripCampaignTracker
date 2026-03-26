namespace DripsCampaignTracker.Entity;

public class Lead
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }

    public List<CampaignLead> CampaignLeads { get; set; } = [];
    public List<Conversation> Conversations { get; set; } = [];
}
