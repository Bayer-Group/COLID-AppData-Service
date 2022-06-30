using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Common.AutoMapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<ConsumerGroup, ConsumerGroupDto>();
            CreateMap<ConsumerGroupDto, ConsumerGroup>();

            CreateMap<SearchFilterDataMarketplace, SearchFilterDataMarketplaceDto>();
            CreateMap<SearchFilterDataMarketplaceDto, SearchFilterDataMarketplace>();

            CreateMap<StoredQueryDto, StoredQuery>();
            CreateMap<StoredQuery, StoredQueryDto>();

            CreateMap<ColidEntrySubscription, ColidEntrySubscriptionDto>();
            CreateMap<ColidEntrySubscriptionDto, ColidEntrySubscription>();

            CreateMap<MessageConfig, MessageConfigDto>();
            CreateMap<MessageConfigDto, MessageConfig>();

            CreateMap<MessageTemplate, MessageTemplateDto>();
            CreateMap<MessageTemplateDto, MessageTemplate>();

            CreateMap<Message, MessageDto>();
            CreateMap<MessageDto, Message>();
            CreateMap<Message, MessageUserDto>();
            CreateMap<MessageUserDto, Message>();

            CreateMap<Microsoft.Graph.User, AdUser>().ForMember(dest => dest.Cwid, opt => opt.MapFrom(src => src.MailNickname));
            CreateMap<Microsoft.Graph.Group, AdGroup>();
        }
    }
}
