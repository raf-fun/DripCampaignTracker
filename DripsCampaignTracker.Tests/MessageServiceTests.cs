using DripsCampaignTracker.Data;
using DripsCampaignTracker.Entity;
using DripsCampaignTracker.Enums;
using DripsCampaignTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DripsCampaignTracker.Tests
{

    public class MessageServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private MessageService CreateService(AppDbContext context, IAIService aiService)
        {
            var notificationLogger = new Mock<ILogger<NotificationService>>();
            var notificationService = new NotificationService(notificationLogger.Object);
            var messageLogger = new Mock<ILogger<MessageService>>();
            return new MessageService(context, aiService, notificationService, messageLogger.Object);
        }

        private IAIService CreateMockAIService(Classification classification, string response)
        {
            var mock = new Mock<IAIService>();
            mock.Setup(a => a.ClassifyReplyAsync(It.IsAny<string>())).ReturnsAsync(classification);
            mock.Setup(a => a.GenerateResponseAsync(It.IsAny<Classification>(), It.IsAny<string>())).ReturnsAsync(response);
            return mock.Object;
        }

        private async Task<Conversation> SeedConversationAsync(AppDbContext context, ConversationStatus status, int followUpCount = 0, int daysAgoContacted = 5)
        {
            var employee = new Employee { Id = 1, Name = "Bruce Wayne-Marketer", Phone = "+11111111111", Role = EmployeeRole.Marketer };
            var manager = new Employee { Id = 2, Name = "Diana Prince-Manager", Phone = "+12222222222", Role = EmployeeRole.Manager };
            var lead = new Lead { Id = 1, Name = "Clark Yes-FirstContact", Phone = "+13001000001" };
            var campaign = new Campaign
            {
                Id = 1,
                Name = "Test Campaign",
                GoalTarget = 4,
                CooldownDays = 2,
                AutoClose = false,
                Status = CampaignStatus.Active,
                MarketerId = 1,
                ManagerId = 2
            };
            var conversation = new Conversation
            {
                Id = 1,
                CampaignId = 1,
                LeadId = 1,
                Status = status,
                FollowUpCount = followUpCount,
                LastContactedDate = DateTime.UtcNow.AddDays(-daysAgoContacted),
                CreatedDate = DateTime.UtcNow.AddDays(-10)
            };

            context.Employees.AddRange(employee, manager);
            context.Leads.Add(lead);
            context.Campaigns.Add(campaign);
            context.Conversations.Add(conversation);
            await context.SaveChangesAsync();

            return conversation;
        }

        [Fact]
        public async Task ProcessIncomingReply_YesReply_SetsConversationToCompleted()
        {
            var context = CreateInMemoryContext();
            await SeedConversationAsync(context, ConversationStatus.Active);
            var aiService = CreateMockAIService(Classification.Yes, "We are so excited to have you!");
            var service = CreateService(context, aiService);

            await service.ProcessIncomingReplyAsync(1, "Yes I will be there!");

            var conversation = await context.Conversations.FindAsync(1);
            Assert.Equal(ConversationStatus.Completed, conversation!.Status);
        }
    }
}
