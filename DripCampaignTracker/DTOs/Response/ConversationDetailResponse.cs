namespace DripsCampaignTracker.DTOs.Response
{
    public class ConversationDetailResponse
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public string LeadName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int FollowUpCount { get; set; }
        public DateTime LastContactedDate { get; set; }
        public int CooldownDays { get; set; }
    }
}
