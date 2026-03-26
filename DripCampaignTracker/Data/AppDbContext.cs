using Microsoft.EntityFrameworkCore;
using DripCampaignTracker.Entity;
using DripCampaignTracker.Enums;

namespace DripCampaignTracker.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<CampaignLead> CampaignLeads { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Employee.Role as string
        modelBuilder.Entity<Employee>()
            .Property(e => e.Role)
            .HasConversion<string>();

        #region Campaign Configuration
        // Configure Campaign.Status as string
        modelBuilder.Entity<Campaign>()
            .Property(c => c.Status)
            .HasConversion<string>();

        // Campaign relationships to Employee
        modelBuilder.Entity<Campaign>()
            .HasOne(c => c.Marketer)
            .WithMany(e => e.CampaignsAsMarketer)
            .HasForeignKey(c => c.MarketerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Campaign>()
            .HasOne(c => c.Manager)
            .WithMany(e => e.CampaignsAsManager)
            .HasForeignKey(c => c.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion

        #region Conversation Configuration
        // Configure Conversation.Status as string
        modelBuilder.Entity<Conversation>()
            .Property(c => c.Status)
            .HasConversion<string>();

        // Conversation relationships
        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Campaign)
            .WithMany(ca => ca.Conversations)
            .HasForeignKey(c => c.CampaignId);

        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Lead)
            .WithMany(l => l.Conversations)
            .HasForeignKey(c => c.LeadId);
        #endregion

        // Configure Message.SenderType as string
        modelBuilder.Entity<Message>()
            .Property(m => m.SenderType)
            .HasConversion<string>();

        #region Message Configuration
        // Configure Message.Classification as string
        modelBuilder.Entity<Message>()
            .Property(m => m.Classification)
            .HasConversion<string>();

        // Message relationships
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId);
        #endregion

        #region CampaignLead Configuration
        // Configure CampaignLead composite key
        modelBuilder.Entity<CampaignLead>()
            .HasKey(cl => new { cl.CampaignId, cl.LeadId });

        // CampaignLead relationships
        modelBuilder.Entity<CampaignLead>()
            .HasOne(cl => cl.Campaign)
            .WithMany(c => c.CampaignLeads)
            .HasForeignKey(cl => cl.CampaignId);

        modelBuilder.Entity<CampaignLead>()
            .HasOne(cl => cl.Lead)
            .WithMany(l => l.CampaignLeads)
            .HasForeignKey(cl => cl.LeadId);
        #endregion

    }
}
