using AutoMapper;
using DripCampaignTracker.DTOs.Response;
using DripCampaignTracker.Entity;
using DripCampaignTracker.Enums;

namespace DripCampaignTracker
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Campaign, CampaignSummaryResponse>()
                .ForMember(dest => dest.Status, 
                    config => config.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.YesCount, 
                    config => config.MapFrom(src => src.Conversations.Count(c => c.Status == ConversationStatus.Completed)));

            CreateMap<Campaign, CampaignDetailResponse>()
                .ForMember(dest => dest.Status, 
                    config => config.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.YesCount, 
                    config => config.MapFrom(src => src.Conversations.Count(c => c.Status == ConversationStatus.Completed)))
                .ForMember(dest => dest.Conversations, 
                    config => config.MapFrom(src => src.Conversations));

            CreateMap<Conversation, ConversationSummaryResponse>()
                .ForMember(dest => dest.Status, config => config.MapFrom(src => src.Status.ToString()));

            CreateMap<Message, MessageResponse>();
        }
    }
}
