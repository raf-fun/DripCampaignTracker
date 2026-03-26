using DripsCampaignTracker.Entity;
using DripsCampaignTracker.Enums;

namespace DripsCampaignTracker.Data
{
    /*
     * SEED DATA LEGEND
     * ================
     * 
     * EMPLOYEES
     * ---------
     * Bruce Wayne-Marketer         - Role: Marketer, owns both campaigns
     * Diana Prince-Manager         - Role: Manager, receives milestone SMS on both campaigns
     * 
     * LEADS
     * -----
     * Campaign 1 - Spring Outreach (Goal: 7, CooldownDays: 2, AutoClose: false)
     * 
     * Clark Yes-FirstContact           - Replied Yes on first contact. Conversation complete.
     * Hal No-FirstContact              - Replied No on first contact. Opted out.
     * Dinah Ambiguous-EligibleFollowUp - Replied Ambiguous. 0 follow ups used. Eligible to follow up now.
     * Barry Ambiguous-MaxFollowUps     - Replied Ambiguous. 2 follow ups used. Cannot follow up again.
     * Kara NoResponse-Eligible         - Never responded. 0 follow ups used. Eligible to follow up now.
     * Oliver NoResponse-MaxFollowUps   - Never responded. 2 follow ups used. Cannot follow up again.
     * Zatanna NoResponse-CooldownActive- Never responded. 1 follow up used. Cooldown not yet passed.
     * Victor NoResponse-Untouched      - Never responded. 0 follow ups. First contact not yet sent.
     * Arthur NoResponse-Untouched      - Never responded. 0 follow ups. First contact not yet sent.
     * John NoResponse-Untouched        - Never responded. 0 follow ups. First contact not yet sent.
     * 
     * Campaign 2 - Summer Push (Goal: 8, CooldownDays: 2, AutoClose: true)
     * Sitting at 25% (2 of 8 Yes) — reply Yes on remaining leads to hit 50%, 75%, 100% live
     * 
     * Selina Yes-Seeded1           - Already Yes. Counts toward 25% milestone.
     * Dick Yes-Seeded2             - Already Yes. Counts toward 25% milestone.
     * Wally ReplyYes-For50         - No response. Reply Yes live to push toward 50%.
     * Barbara ReplyYes-For50       - No response. Reply Yes live to push toward 50%.
     * Jason ReplyYes-For75         - No response. Reply Yes live to push toward 75%.
     * Donna ReplyYes-For75         - No response. Reply Yes live to push toward 75%.
     * Roy ReplyYes-For100          - No response. Reply Yes live to push toward 100%.
     * Mera ReplyYes-For100         - No response. Reply Yes live to push toward 100%.
     */

    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Employees
            var marketer = new Employee
            {
                Id = 1,
                Name = "Bruce Wayne-Marketer",
                Phone = "+11111111111",
                Role = EmployeeRole.Marketer,
                CreatedDate = DateTime.UtcNow.AddDays(-30)
            };

            var manager = new Employee
            {
                Id = 2,
                Name = "Diana Prince-Manager",
                Phone = "+12222222222",
                Role = EmployeeRole.Manager,
                CreatedDate = DateTime.UtcNow.AddDays(-30)
            };

            context.Employees.AddRange(marketer, manager);

            // -------------------------
            // LEADS - Campaign 1
            // -------------------------
            var clark = new Lead { Id = 1, Name = "Clark Yes-FirstContact", Phone = "+13001000001", CreatedDate = DateTime.UtcNow.AddDays(-20) };
            var hal = new Lead { Id = 2, Name = "Hal No-FirstContact", Phone = "+13001000002", CreatedDate = DateTime.UtcNow.AddDays(-20) };
            var dinah = new Lead { Id = 3, Name = "Dinah Ambiguous-EligibleFollowUp", Phone = "+13001000003", CreatedDate = DateTime.UtcNow.AddDays(-20) };
            var barry = new Lead { Id = 4, Name = "Barry Ambiguous-MaxFollowUps", Phone = "+13001000004", CreatedDate = DateTime.UtcNow.AddDays(-20) };
            var kara = new Lead { Id = 5, Name = "Kara NoResponse-Eligible", Phone = "+13001000005", CreatedDate = DateTime.UtcNow.AddDays(-20) };
            var oliver = new Lead { Id = 6, Name = "Oliver NoResponse-MaxFollowUps", Phone = "+13001000006", CreatedDate = DateTime.UtcNow.AddDays(-20) };
            var zatanna = new Lead { Id = 7, Name = "Zatanna NoResponse-CooldownActive", Phone = "+13001000007", CreatedDate = DateTime.UtcNow.AddDays(-20) };
            var victor = new Lead { Id = 8, Name = "Victor NoResponse-Untouched", Phone = "+13001000008", CreatedDate = DateTime.UtcNow.AddDays(-20) };
            var arthur = new Lead { Id = 9, Name = "Arthur NoResponse-Untouched", Phone = "+13001000009", CreatedDate = DateTime.UtcNow.AddDays(-20) };
            var john = new Lead { Id = 10, Name = "John NoResponse-Untouched", Phone = "+13001000010", CreatedDate = DateTime.UtcNow.AddDays(-20) };

            // LEADS - Campaign 2
            var selina = new Lead { Id = 11, Name = "Selina Yes-Seeded1", Phone = "+13002000001", CreatedDate = DateTime.UtcNow.AddDays(-15) };
            var dick = new Lead { Id = 12, Name = "Dick Yes-Seeded2", Phone = "+13002000002", CreatedDate = DateTime.UtcNow.AddDays(-15) };
            var wally = new Lead { Id = 13, Name = "Wally ReplyYes-For50", Phone = "+13002000003", CreatedDate = DateTime.UtcNow.AddDays(-15) };
            var barbara = new Lead { Id = 14, Name = "Barbara ReplyYes-For50", Phone = "+13002000004", CreatedDate = DateTime.UtcNow.AddDays(-15) };
            var jason = new Lead { Id = 15, Name = "Jason ReplyYes-For75", Phone = "+13002000005", CreatedDate = DateTime.UtcNow.AddDays(-15) };
            var donna = new Lead { Id = 16, Name = "Donna ReplyYes-For75", Phone = "+13002000006", CreatedDate = DateTime.UtcNow.AddDays(-15) };
            var roy = new Lead { Id = 17, Name = "Roy ReplyYes-For100", Phone = "+13002000007", CreatedDate = DateTime.UtcNow.AddDays(-15) };
            var mera = new Lead { Id = 18, Name = "Mera ReplyYes-For100", Phone = "+13002000008", CreatedDate = DateTime.UtcNow.AddDays(-15) };

            context.Leads.AddRange(clark, hal, dinah, barry, kara, oliver, zatanna, victor, arthur, john,
                                   selina, dick, wally, barbara, jason, donna, roy, mera);

            // -------------------------
            // CAMPAIGNS
            // -------------------------
            var campaign1 = new Campaign
            {
                Id = 1,
                Name = "Spring Outreach",
                GoalTarget = 7,
                CooldownDays = 2,
                AutoClose = false,
                Status = CampaignStatus.Active,
                MarketerId = 1,
                ManagerId = 2,
                CreatedDate = DateTime.UtcNow.AddDays(-20)
            };

            var campaign2 = new Campaign
            {
                Id = 2,
                Name = "Summer Push",
                GoalTarget = 8,
                CooldownDays = 2,
                AutoClose = true,
                Status = CampaignStatus.Active,
                MarketerId = 1,
                ManagerId = 2,
                CreatedDate = DateTime.UtcNow.AddDays(-15)
            };

            context.Campaigns.AddRange(campaign1, campaign2);

            // -------------------------
            // CAMPAIGN LEADS
            // -------------------------
            context.CampaignLeads.AddRange(
                new CampaignLead { CampaignId = 1, LeadId = 1 },
                new CampaignLead { CampaignId = 1, LeadId = 2 },
                new CampaignLead { CampaignId = 1, LeadId = 3 },
                new CampaignLead { CampaignId = 1, LeadId = 4 },
                new CampaignLead { CampaignId = 1, LeadId = 5 },
                new CampaignLead { CampaignId = 1, LeadId = 6 },
                new CampaignLead { CampaignId = 1, LeadId = 7 },
                new CampaignLead { CampaignId = 1, LeadId = 8 },
                new CampaignLead { CampaignId = 1, LeadId = 9 },
                new CampaignLead { CampaignId = 1, LeadId = 10 },
                new CampaignLead { CampaignId = 2, LeadId = 11 },
                new CampaignLead { CampaignId = 2, LeadId = 12 },
                new CampaignLead { CampaignId = 2, LeadId = 13 },
                new CampaignLead { CampaignId = 2, LeadId = 14 },
                new CampaignLead { CampaignId = 2, LeadId = 15 },
                new CampaignLead { CampaignId = 2, LeadId = 16 },
                new CampaignLead { CampaignId = 2, LeadId = 17 },
                new CampaignLead { CampaignId = 2, LeadId = 18 }
            );

            // -------------------------
            // CONVERSATIONS & MESSAGES
            // -------------------------

            // Clark - Yes on first contact
            context.Conversations.Add(new Conversation
            {
                Id = 1,
                CampaignId = 1,
                LeadId = 1,
                Status = ConversationStatus.Completed,
                FollowUpCount = 0,
                LastContactedDate = DateTime.UtcNow.AddDays(-10),
                CreatedDate = DateTime.UtcNow.AddDays(-10)
            });
            context.Messages.AddRange(
                new Message { Id = 1, ConversationId = 1, Content = "Hi Clark, are you interested in coming in?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-10) },
                new Message { Id = 2, ConversationId = 1, Content = "Yes I would love to come in!", SenderType = SenderType.Lead, Classification = Classification.Yes, SentDate = DateTime.UtcNow.AddDays(-10).AddMinutes(30) },
                new Message { Id = 3, ConversationId = 1, Content = "That is amazing Clark! We are so excited to see you. We will be in touch shortly.", SenderType = SenderType.AI, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-10).AddMinutes(31) }
            );

            // Hal - No on first contact
            context.Conversations.Add(new Conversation
            {
                Id = 2,
                CampaignId = 1,
                LeadId = 2,
                Status = ConversationStatus.OptedOut,
                FollowUpCount = 0,
                LastContactedDate = DateTime.UtcNow.AddDays(-9),
                CreatedDate = DateTime.UtcNow.AddDays(-9)
            });
            context.Messages.AddRange(
                new Message { Id = 4, ConversationId = 2, Content = "Hi Hal, are you interested in coming in?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-9) },
                new Message { Id = 5, ConversationId = 2, Content = "No thanks, not interested.", SenderType = SenderType.Lead, Classification = Classification.No, SentDate = DateTime.UtcNow.AddDays(-9).AddMinutes(20) },
                new Message { Id = 6, ConversationId = 2, Content = "Thank you for letting us know Hal. We appreciate your time.", SenderType = SenderType.AI, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-9).AddMinutes(21) }
            );

            // Dinah - Ambiguous, eligible for follow up
            context.Conversations.Add(new Conversation
            {
                Id = 3,
                CampaignId = 1,
                LeadId = 3,
                Status = ConversationStatus.NeedsFollowUp,
                FollowUpCount = 0,
                LastContactedDate = DateTime.UtcNow.AddDays(-5),
                CreatedDate = DateTime.UtcNow.AddDays(-5)
            });
            context.Messages.AddRange(
                new Message { Id = 7, ConversationId = 3, Content = "Hi Dinah, are you interested in coming in?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-5) },
                new Message { Id = 8, ConversationId = 3, Content = "Maybe, I am not sure yet.", SenderType = SenderType.Lead, Classification = Classification.Ambiguous, SentDate = DateTime.UtcNow.AddDays(-5).AddMinutes(45) },
                new Message { Id = 9, ConversationId = 3, Content = "No worries Dinah! We will follow back up with you soon.", SenderType = SenderType.AI, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-5).AddMinutes(46) }
            );

            // Barry - Ambiguous, max follow ups reached
            context.Conversations.Add(new Conversation
            {
                Id = 4,
                CampaignId = 1,
                LeadId = 4,
                Status = ConversationStatus.NeedsFollowUp,
                FollowUpCount = 2,
                LastContactedDate = DateTime.UtcNow.AddDays(-3),
                CreatedDate = DateTime.UtcNow.AddDays(-10)
            });
            context.Messages.AddRange(
                new Message { Id = 10, ConversationId = 4, Content = "Hi Barry, are you interested in coming in?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-10) },
                new Message { Id = 11, ConversationId = 4, Content = "I am not sure.", SenderType = SenderType.Lead, Classification = Classification.Ambiguous, SentDate = DateTime.UtcNow.AddDays(-10).AddHours(2) },
                new Message { Id = 12, ConversationId = 4, Content = "No worries Barry! We will follow back up with you soon.", SenderType = SenderType.AI, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-10).AddHours(2).AddMinutes(1) },
                new Message { Id = 13, ConversationId = 4, Content = "Hey Barry just checking back in, are you interested?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-6) },
                new Message { Id = 14, ConversationId = 4, Content = "Still not sure honestly.", SenderType = SenderType.Lead, Classification = Classification.Ambiguous, SentDate = DateTime.UtcNow.AddDays(-6).AddHours(1) },
                new Message { Id = 15, ConversationId = 4, Content = "No worries Barry! We will follow back up with you soon.", SenderType = SenderType.AI, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-6).AddHours(1).AddMinutes(1) },
                new Message { Id = 16, ConversationId = 4, Content = "Hi Barry, last chance to join us!", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-3) },
                new Message { Id = 17, ConversationId = 4, Content = "Hmm maybe later.", SenderType = SenderType.Lead, Classification = Classification.Ambiguous, SentDate = DateTime.UtcNow.AddDays(-3).AddHours(3) },
                new Message { Id = 18, ConversationId = 4, Content = "No worries Barry! We will follow back up with you soon.", SenderType = SenderType.AI, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-3).AddHours(3).AddMinutes(1) }
            );

            // Kara - No response, eligible for follow up
            context.Conversations.Add(new Conversation
            {
                Id = 5,
                CampaignId = 1,
                LeadId = 5,
                Status = ConversationStatus.NeedsFollowUp,
                FollowUpCount = 0,
                LastContactedDate = DateTime.UtcNow.AddDays(-5),
                CreatedDate = DateTime.UtcNow.AddDays(-5)
            });
            context.Messages.Add(
                new Message { Id = 19, ConversationId = 5, Content = "Hi Kara, are you interested in coming in?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-5) }
            );

            // Oliver - No response, max follow ups reached
            context.Conversations.Add(new Conversation
            {
                Id = 6,
                CampaignId = 1,
                LeadId = 6,
                Status = ConversationStatus.NeedsFollowUp,
                FollowUpCount = 2,
                LastContactedDate = DateTime.UtcNow.AddDays(-3),
                CreatedDate = DateTime.UtcNow.AddDays(-12)
            });
            context.Messages.AddRange(
                new Message { Id = 20, ConversationId = 6, Content = "Hi Oliver, are you interested in coming in?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-12) },
                new Message { Id = 21, ConversationId = 6, Content = "Hey Oliver just following up!", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-7) },
                new Message { Id = 22, ConversationId = 6, Content = "Hi Oliver, last chance to join us!", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-3) }
            );

            // Zatanna - No response, cooldown active (contacted 1 day ago, cooldown is 2 days)
            context.Conversations.Add(new Conversation
            {
                Id = 7,
                CampaignId = 1,
                LeadId = 7,
                Status = ConversationStatus.NeedsFollowUp,
                FollowUpCount = 1,
                LastContactedDate = DateTime.UtcNow.AddDays(-1),
                CreatedDate = DateTime.UtcNow.AddDays(-8)
            });
            context.Messages.AddRange(
                new Message { Id = 23, ConversationId = 7, Content = "Hi Zatanna, are you interested in coming in?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-8) },
                new Message { Id = 24, ConversationId = 7, Content = "Hey Zatanna just following up!", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-1) }
            );

            // Victor, Arthur, John - Never contacted
            context.Conversations.AddRange(
                new Conversation { Id = 8, CampaignId = 1, LeadId = 8, Status = ConversationStatus.Active, FollowUpCount = 0, LastContactedDate = DateTime.UtcNow.AddDays(-20), CreatedDate = DateTime.UtcNow.AddDays(-20) },
                new Conversation { Id = 9, CampaignId = 1, LeadId = 9, Status = ConversationStatus.Active, FollowUpCount = 0, LastContactedDate = DateTime.UtcNow.AddDays(-20), CreatedDate = DateTime.UtcNow.AddDays(-20) },
                new Conversation { Id = 10, CampaignId = 1, LeadId = 10, Status = ConversationStatus.Active, FollowUpCount = 0, LastContactedDate = DateTime.UtcNow.AddDays(-20), CreatedDate = DateTime.UtcNow.AddDays(-20) }
            );

            // -------------------------
            // CAMPAIGN 2 - Summer Push
            // -------------------------

            // Selina - Already Yes
            context.Conversations.Add(new Conversation
            {
                Id = 11,
                CampaignId = 2,
                LeadId = 11,
                Status = ConversationStatus.Completed,
                FollowUpCount = 0,
                LastContactedDate = DateTime.UtcNow.AddDays(-10),
                CreatedDate = DateTime.UtcNow.AddDays(-10)
            });
            context.Messages.AddRange(
                new Message { Id = 25, ConversationId = 11, Content = "Hi Selina, are you ready to join us this summer?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-10) },
                new Message { Id = 26, ConversationId = 11, Content = "Yes absolutely count me in!", SenderType = SenderType.Lead, Classification = Classification.Yes, SentDate = DateTime.UtcNow.AddDays(-10).AddMinutes(15) },
                new Message { Id = 27, ConversationId = 11, Content = "That is amazing Selina! We are so excited to have you. See you soon!", SenderType = SenderType.AI, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-10).AddMinutes(16) }
            );

            // Dick - Already Yes
            context.Conversations.Add(new Conversation
            {
                Id = 12,
                CampaignId = 2,
                LeadId = 12,
                Status = ConversationStatus.Completed,
                FollowUpCount = 0,
                LastContactedDate = DateTime.UtcNow.AddDays(-9),
                CreatedDate = DateTime.UtcNow.AddDays(-9)
            });
            context.Messages.AddRange(
                new Message { Id = 28, ConversationId = 12, Content = "Hi Dick, are you ready to join us this summer?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-9) },
                new Message { Id = 29, ConversationId = 12, Content = "For sure, I am in!", SenderType = SenderType.Lead, Classification = Classification.Yes, SentDate = DateTime.UtcNow.AddDays(-9).AddMinutes(10) },
                new Message { Id = 30, ConversationId = 12, Content = "That is amazing Dick! We are so excited to have you. See you soon!", SenderType = SenderType.AI, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-9).AddMinutes(11) }
            );

            // Wally, Barbara, Jason, Donna, Roy, Mera - No response, ready to reply Yes live
            context.Conversations.AddRange(
                new Conversation { Id = 13, CampaignId = 2, LeadId = 13, Status = ConversationStatus.Active, FollowUpCount = 0, LastContactedDate = DateTime.UtcNow.AddDays(-8), CreatedDate = DateTime.UtcNow.AddDays(-8) },
                new Conversation { Id = 14, CampaignId = 2, LeadId = 14, Status = ConversationStatus.Active, FollowUpCount = 0, LastContactedDate = DateTime.UtcNow.AddDays(-8), CreatedDate = DateTime.UtcNow.AddDays(-8) },
                new Conversation { Id = 15, CampaignId = 2, LeadId = 15, Status = ConversationStatus.Active, FollowUpCount = 0, LastContactedDate = DateTime.UtcNow.AddDays(-8), CreatedDate = DateTime.UtcNow.AddDays(-8) },
                new Conversation { Id = 16, CampaignId = 2, LeadId = 16, Status = ConversationStatus.Active, FollowUpCount = 0, LastContactedDate = DateTime.UtcNow.AddDays(-8), CreatedDate = DateTime.UtcNow.AddDays(-8) },
                new Conversation { Id = 17, CampaignId = 2, LeadId = 17, Status = ConversationStatus.Active, FollowUpCount = 0, LastContactedDate = DateTime.UtcNow.AddDays(-8), CreatedDate = DateTime.UtcNow.AddDays(-8) },
                new Conversation { Id = 18, CampaignId = 2, LeadId = 18, Status = ConversationStatus.Active, FollowUpCount = 0, LastContactedDate = DateTime.UtcNow.AddDays(-8), CreatedDate = DateTime.UtcNow.AddDays(-8) }
            );
            context.Messages.AddRange(
                new Message { Id = 31, ConversationId = 13, Content = "Hi Wally, are you ready to join us this summer?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-8) },
                new Message { Id = 32, ConversationId = 14, Content = "Hi Barbara, are you ready to join us this summer?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-8) },
                new Message { Id = 33, ConversationId = 15, Content = "Hi Jason, are you ready to join us this summer?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-8) },
                new Message { Id = 34, ConversationId = 16, Content = "Hi Donna, are you ready to join us this summer?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-8) },
                new Message { Id = 35, ConversationId = 17, Content = "Hi Roy, are you ready to join us this summer?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-8) },
                new Message { Id = 36, ConversationId = 18, Content = "Hi Mera, are you ready to join us this summer?", SenderType = SenderType.Marketer, Classification = Classification.None, SentDate = DateTime.UtcNow.AddDays(-8) }
            );

            context.SaveChanges();
        }
    }
}