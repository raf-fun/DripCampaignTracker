using AutoMapper;
using DripsCampaignTracker.Data;
using DripsCampaignTracker.DTOs;
using DripsCampaignTracker.DTOs.Response;
using DripsCampaignTracker.Entity;
using DripsCampaignTracker.Enums;
using DripsCampaignTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DripsCampaignTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationController(AppDbContext dbContext, IMapper mapper, MessageService messageService) : ControllerBase
{

    [HttpGet("{id}/messages")]
    public async Task<IActionResult> GetConversationMessages(int id)
    {
        var conversation = await dbContext.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conversation == null)
        {
            return NotFound();
        }

        var messages = conversation.Messages.OrderBy(m => m.SentDate).ToList();
        return Ok(mapper.Map<List<MessageResponse>>(messages));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ConversationDetailResponse>> GetConversation(int id)
    {
        var conversation = await dbContext.Conversations
            .Include(c => c.Lead)
            .Include(c => c.Campaign)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conversation == null || conversation.Campaign == null)
            return NotFound();

        return Ok(new ConversationDetailResponse
        {
            Id = conversation.Id,
            LeadId = conversation.LeadId,
            LeadName = conversation.Lead?.Name ?? "Unknown",
            Status = conversation.Status.ToString(),
            FollowUpCount = conversation.FollowUpCount,
            LastContactedDate = conversation.LastContactedDate,
            CooldownDays = conversation.Campaign.CooldownDays
        });
    }

    [HttpPost("{id}/messages")]
    public async Task<IActionResult> SendMessage(int id, [FromBody] CreateMessageRequest request)
    {
        var conversation = await dbContext.Conversations.FirstOrDefaultAsync(c => c.Id == id);

        if (conversation == null)
        {
            return NotFound();
        }

        if (request.SenderType == SenderType.Lead)
        {
            //Go through AI
            await messageService.ProcessIncomingReplyAsync(id, request.Content);
        }
        else
        {
            //
            await messageService.SendFollowUpAsync(id, request.Content);
        }

        return Ok();
    }
}
