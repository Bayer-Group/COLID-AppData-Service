using System;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Interface;
using COLID.Cache.Services;

namespace COLID.AppDataService.Services.Implementation
{
    public class WelcomeMessageService : GenericService<WelcomeMessage, int>, IWelcomeMessageService
    {
        private readonly IWelcomeMessageRepository _wmRepo;
        private readonly ICacheService _cache;

        public WelcomeMessageService(IWelcomeMessageRepository wmRepo, ICacheService cache) : base(wmRepo)
        {
            _wmRepo = wmRepo;
            _cache = cache;
        }

        public WelcomeMessage GetWelcomeMessageEditor()
        {
            return GetWelcomeMessage(ColidType.Editor);
        }

        public WelcomeMessage GetWelcomeMessageDataMarketplace()
        {
            return GetWelcomeMessage(ColidType.DataMarketplace);
        }

        private WelcomeMessage GetWelcomeMessage(ColidType type)
        {
            return _cache.GetOrAdd(type.ToString(), () => _wmRepo.GetOne(type));
        }

        public WelcomeMessage UpdateWelcomeMessageEditor(string content)
        {
            return UpdateWelcomeMessage(ColidType.Editor, content);
        }

        public WelcomeMessage UpdateWelcomeMessageDataMarketplace(string content)
        {
            return UpdateWelcomeMessage(ColidType.DataMarketplace, content);
        }

        private WelcomeMessage UpdateWelcomeMessage(ColidType type, string content)
        {
            Guard.IsNotEmpty(content);
            var entity = GetWelcomeMessage(type);
            entity.Content = content;

            return _cache.Update(type.ToString(), () => _wmRepo.Update(entity));
        }
        
    }
}
