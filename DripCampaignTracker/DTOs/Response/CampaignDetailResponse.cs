namespace DripsCampaignTracker.DTOs.Response
{
    public class CampaignDetailResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GoalTarget { get; set; }
        public int CooldownDays { get; set; }
        public bool AutoClose { get; set; }
        public string Status { get; set; } = string.Empty;
        public int MarketerId { get; set; }
        public int ManagerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int YesCount { get; set; }
        public List<ConversationSummaryResponse> Conversations { get; set; } = [];
    }
}
