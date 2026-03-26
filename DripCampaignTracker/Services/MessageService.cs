using DripCampaignTracker.Data;
using DripCampaignTracker.Entity;
using DripCampaignTracker.Enums;
using Microsoft.EntityFrameworkCore;

namespace DripCampaignTracker.Services
{
    public class MessageService(
        AppDbContext context,
        AIService aiService,
        NotificationService notificationService,
        ILogger<MessageService> logger)
    {
        public async Task ProcessIncomingReplyAsync(int conversationId, string messageContent)
        {
            var conversation = await context.Conversations
                .Include(c => c.Campaign)
                .Include(c => c.Lead)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                logger.LogWarning($"Conversation {conversationId} not found.");
                return;
            }

            if (conversation.Status == ConversationStatus.OptedOut)
            {
                logger.LogInformation($"Conversation {conversationId} is opted out. Ignoring reply.");
                return;
            }

            if (conversation.Lead == null)
            {
                logger.LogWarning($"Conversation {conversationId} Lead not found.");
                return;
            }

            // Classify the lead reply
            var classification = await aiService.ClassifyReplyAsync(messageContent);
            logger.LogInformation($"Reply classified as {classification} for conversation {conversationId}.");

            // Save the lead's incoming message
            var incomingMessage = new Message
            {
                ConversationId = conversationId,
                Content = messageContent,
                SenderType = SenderType.Lead,
                Classification = classification,
                SentDate = DateTime.UtcNow
            };
            context.Messages.Add(incomingMessage);

            // Update conversation status based on classification
            switch (classification)
            {
                case Classification.Yes:
                    conversation.Status = ConversationStatus.Completed;
                    break;
                case Classification.No:
                    conversation.Status = ConversationStatus.OptedOut;
                    break;
                default:
                    conversation.Status = ConversationStatus.NeedsFollowUp;
                    break;
            }

            conversation.LastContactedDate = DateTime.UtcNow;
            await context.SaveChangesAsync();

            // Generate and save AI response
            var responseContent = await aiService.GenerateResponseAsync(classification, conversation.Lead.Name);
            var aiResponse = new Message
            {
                ConversationId = conversationId,
                Content = responseContent,
                SenderType = SenderType.AI,
                Classification = Classification.None,
                SentDate = DateTime.UtcNow
            };
            context.Messages.Add(aiResponse);
            await context.SaveChangesAsync();

            // Check if send milestones
            if (classification == Classification.Yes)
            {
                await CheckMilestonesAsync(conversation.CampaignId);
            }
        }

        public async Task SendFollowUpAsync(int conversationId, string messageContent)
        {
            var conversation = await context.Conversations
                .Include(c => c.Campaign)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                logger.LogWarning($"Conversation {conversationId} not found.");
                return;
            }
            if (conversation.Campaign == null)
            {
                logger.LogWarning($"Conversation {conversationId}  campaign not found.");
                return;
            }

            // Max Follow Ups Check
            if (conversation.FollowUpCount >= 2)
            {
                logger.LogWarning($"Conversation {conversationId} has reached max follow ups.");
                return;
            }

            if (conversation.Status == ConversationStatus.OptedOut)
            {
                logger.LogWarning($"Conversation {conversationId} is opted out. Cannot follow up.");
                return;
            }

            var daysSinceLastContact = (DateTime.UtcNow - conversation.LastContactedDate).TotalDays;
            if (daysSinceLastContact < conversation.Campaign.CooldownDays)
            {
                logger.LogWarning($"Conversation {conversationId} is still in cooldown period.");
                return;
            }

            // Save follow up message
            var message = new Message
            {
                ConversationId = conversationId,
                Content = messageContent,
                SenderType = SenderType.Marketer,
                Classification = Classification.None,
                SentDate = DateTime.UtcNow
            };

            context.Messages.Add(message);
            conversation.FollowUpCount++;
            conversation.LastContactedDate = DateTime.UtcNow;

            await context.SaveChangesAsync();
            logger.LogInformation($"Follow up sent for conversation {conversationId}. Follow up count: {conversation.FollowUpCount}.");
        }

        private async Task CheckMilestonesAsync(int campaignId)
        {
            var campaign = await context.Campaigns
                .Include(c => c.Conversations)
                .FirstOrDefaultAsync(c => c.Id == campaignId);

            if (campaign == null) return;

            var yesCount = campaign.Conversations.Count(c => c.Status == ConversationStatus.Completed);
            var percentage = (double)yesCount / campaign.GoalTarget * 100;

            var milestones = new[] { 25, 50, 75, 100 };
            foreach (var milestone in milestones)
            {
                if (percentage >= milestone)
                {
                    await notificationService.SendMilestoneNotificationAsync(campaign, milestone, yesCount);
                }
            }

            // Auto close if goal reached and auto close is enabled
            if (percentage >= 100 && campaign.AutoClose)
            {
                campaign.Status = CampaignStatus.Closed;
                await context.SaveChangesAsync();
                logger.LogInformation($"Campaign {campaignId} auto closed after reaching goal.");
            }
        }
    }
}
