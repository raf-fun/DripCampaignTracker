using AutoMapper;
using DripCampaignTracker.Data;
using DripCampaignTracker.DTOs;
using DripCampaignTracker.DTOs.Response;
using DripCampaignTracker.Entity;
using DripCampaignTracker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace DripCampaignTracker.Controllers;

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

        return Ok();
    }
}
