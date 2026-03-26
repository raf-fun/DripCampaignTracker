using DripCampaignTracker.Enums;

namespace DripCampaignTracker.Entity;

public class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public EmployeeRole Role { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedDate { get; set; }

    public ICollection<Campaign> CampaignsAsMarketer { get; set; } = [];
    public ICollection<Campaign> CampaignsAsManager { get; set; } = [];
}
