using AutoMapper;
using DripCampaignTracker.Data;
using DripCampaignTracker.DTOs;
using DripCampaignTracker.DTOs.Response;
using DripCampaignTracker.Entity;
using DripCampaignTracker.Enums;
using DripCampaignTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DripCampaignTracker.Controllers;

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
