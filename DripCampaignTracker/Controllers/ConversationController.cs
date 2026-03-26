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
