using AutoMapper;
using DripsCampaignTracker.Data;
using DripsCampaignTracker.DTOs;
using DripsCampaignTracker.DTOs.Response;
using DripsCampaignTracker.Entity;
using DripsCampaignTracker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace DripsCampaignTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController (AppDbContext dbContext, IMapper mapper) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetCampaigns()
    {
        var campaigns = await dbContext.Campaigns
            .Include(c => c.Conversations)
            .ToListAsync();

        return Ok(mapper.Map<List<CampaignSummaryResponse>>(campaigns));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCampaignById(int id)
    {
        var campaign = await dbContext.Campaigns
            .Include(c => c.Conversations)
            .ThenInclude(conv => conv.Lead)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (campaign == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<CampaignDetailResponse>(campaign));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest request)
    {
        if (request.LeadIds.Count < 10)
            return BadRequest("A campaign must have at least 10 leads.");

        var campaign = new Campaign
        {
            Name = request.Name,
            GoalTarget = request.GoalTarget,
            CooldownDays = request.CooldownDays,
            AutoClose = request.AutoClose,
            Status = CampaignStatus.Active,
            MarketerId = request.MarketerId,
            ManagerId = request.ManagerId,
            CreatedDate = DateTime.UtcNow
        };

        dbContext.Campaigns.Add(campaign);
        await dbContext.SaveChangesAsync();

        foreach (var leadId in request.LeadIds)
        {
            dbContext.CampaignLeads.Add(new CampaignLead
            {
                CampaignId = campaign.Id,
                LeadId = leadId
            });

            dbContext.Conversations.Add(new Conversation
            {
                CampaignId = campaign.Id,
                LeadId = leadId,
                Status = ConversationStatus.Active,
                FollowUpCount = 0,
                LastContactedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            });
        }

        await dbContext.SaveChangesAsync();
        return Ok();
    }
}
