using DripsCampaignTracker.Entity;

namespace DripsCampaignTracker.Services
{
    public class NotificationService(ILogger<NotificationService> logger)
    {
        private readonly HashSet<string> sentNotifications = new();

        public Task SendMilestoneNotificationAsync(Campaign campaign, int milestone, int yesCount)
        {
            var key = $"{campaign.Id}-{milestone}";

            if (sentNotifications.Contains(key))
                return Task.CompletedTask;

            sentNotifications.Add(key);

            logger.LogInformation(
                $"[SMS NOTIFICATION] Campaign '{campaign.Name}' has reached {milestone}% of goal. " +
                $"{yesCount}/{campaign.GoalTarget} leads confirmed. Manager (ID: {campaign.ManagerId}) notified.");

            return Task.CompletedTask;
        }
    }
}
