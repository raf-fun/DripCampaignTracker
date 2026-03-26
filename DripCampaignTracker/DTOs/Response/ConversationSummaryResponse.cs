namespace DripCampaignTracker.DTOs.Response
{
    public class ConversationSummaryResponse
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int FollowUpCount { get; set; }
        public DateTime LastContactedDate { get; set; }
    }
}
