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
    /// <summary>
    /// Contains unit tests for the MessageService class, verifying message processing, follow-up logic, and
    /// conversation state transitions.
    /// </summary>
    /// <remarks>These tests use in-memory database contexts and mock dependencies to validate the behavior of
    /// MessageService under various scenarios, including handling of incoming replies and follow-up message sending.
    /// The tests ensure that conversation statuses and message records are updated as expected based on different
    /// inputs and business rules.</remarks>
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

        [Fact]
        public async Task ProcessIncomingReply_OptedOutConversation_IgnoresReply()
        {
            var context = CreateInMemoryContext();
            await SeedConversationAsync(context, ConversationStatus.OptedOut);
            var aiService = CreateMockAIService(Classification.Yes, "We are so excited to have you!");
            var service = CreateService(context, aiService);

            await service.ProcessIncomingReplyAsync(1, "Actually I changed my mind, yes!");

            var messages = await context.Messages.ToListAsync();
            Assert.Empty(messages);
        }

        [Fact]
        public async Task SendFollowUp_CooldownNotPassed_DoesNotSendMessage()
        {
            var context = CreateInMemoryContext();
            await SeedConversationAsync(context, ConversationStatus.NeedsFollowUp, followUpCount: 0, daysAgoContacted: 1);
            var aiService = CreateMockAIService(Classification.Ambiguous, "We will follow up soon.");
            var service = CreateService(context, aiService);

            await service.SendFollowUpAsync(1, "Just checking in!");

            var messages = await context.Messages.ToListAsync();
            Assert.Empty(messages);
        }

        [Fact]
        public async Task SendFollowUp_MaxFollowUpsReached_DoesNotSendMessage()
        {
            var context = CreateInMemoryContext();
            await SeedConversationAsync(context, ConversationStatus.NeedsFollowUp, followUpCount: 2, daysAgoContacted: 5);
            var aiService = CreateMockAIService(Classification.Ambiguous, "We will follow up soon.");
            var service = CreateService(context, aiService);

            await service.SendFollowUpAsync(1, "Just checking in!");

            var messages = await context.Messages.ToListAsync();
            Assert.Empty(messages);
        }

        [Fact]
        public async Task ProcessIncomingReply_NoReply_SetsConversationToOptedOut()
        {
            var context = CreateInMemoryContext();
            await SeedConversationAsync(context, ConversationStatus.Active);
            var aiService = CreateMockAIService(Classification.No, "Thank you for your time.");
            var service = CreateService(context, aiService);

            await service.ProcessIncomingReplyAsync(1, "No thanks not interested.");

            var conversation = await context.Conversations.FindAsync(1);
            Assert.Equal(ConversationStatus.OptedOut, conversation!.Status);
        }

        [Fact]
        public async Task SendFollowUp_ValidConditions_IncrementsFollowUpCount()
        {
            var context = CreateInMemoryContext();
            await SeedConversationAsync(context, ConversationStatus.NeedsFollowUp, followUpCount: 0, daysAgoContacted: 5);
            var aiService = CreateMockAIService(Classification.Ambiguous, "We will follow up soon.");
            var service = CreateService(context, aiService);

            await service.SendFollowUpAsync(1, "Just checking in!");

            var conversation = await context.Conversations.FindAsync(1);
            Assert.Equal(1, conversation!.FollowUpCount);
        }
    }
}
