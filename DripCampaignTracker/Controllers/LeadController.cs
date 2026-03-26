using DripsCampaignTracker.Data;
using DripsCampaignTracker.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DripsCampaignTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Lead>>> GetLeads()
        {
            return Ok(await context.Leads.ToListAsync());
        }
    }
}
