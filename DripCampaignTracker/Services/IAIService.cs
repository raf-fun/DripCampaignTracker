using DripsCampaignTracker.Enums;

namespace DripsCampaignTracker.Services
{
    public interface IAIService
    {
        Task<Classification> ClassifyReplyAsync(string message);
        Task<string> GenerateResponseAsync(Classification classification, string leadName);
    }
}