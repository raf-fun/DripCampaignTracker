using DripsCampaignTracker.Enums;

namespace DripsCampaignTracker.Entity;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }

    public List<Campaign> CampaignsAsMarketer { get; set; } = [];
    public List<Campaign> CampaignsAsManager { get; set; } = [];
}
